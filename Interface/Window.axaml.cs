using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Algorithms;
using Algorithms.Structures;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Domain.Extensions;
using Domain.Structures;
using Interface.Types;
using ScottPlot;
using ScottPlot.Control;
using ScottPlot.Palettes;
using SkiaSharp;
using static System.Linq.Enumerable;
using static Domain.Extensions.EnumerableExtensions;

namespace Interface;

public sealed partial class MainWindow : Window {
  public MainWindow() {
    Interaction = new(this);
    Memory = new(this);
    InitializeComponent();
    InitializeComboBoxes();
    InitializeChart();
    InitializeListeners();
  }

  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;


    Memory.Histories.CollectionChanged += (_, _) => ChartRefresh();

    HistorySlider.PropertyChanged += (_, change) => {
      if (change.Property.Name != "Value") return;

      HistoryText.Text = $"Krok: {Interaction.Step}";
      ChartRefresh();
    };
    RunButton.Click += (_, _) => HandleRunCommand();

    ClearParameterStartIndexButton.Click += (_, _) => ParameterStartIndex.Value = 0;
    ParameterStartIndex.Value = 0;
    ClearParameterRegretButton.Click += (_, _) => ParameterRegret.Value = 2;
    ParameterRegret.Value = 2;
    ClearParameterPopulationSizeButton.Click += (_, _) => ParameterPopulationSize.Value = 1;
    ParameterPopulationSize.Value = 1;
    FindBestButton.Click += (_, _) => {
      ParameterStartIndex.Value = Range((int)ParameterStartIndex.Minimum + 1, (int)ParameterStartIndex.Maximum - 1)
        .MinBy(start => {
          var configuration = new SearchConfiguration {
            Population = Range(0, Interaction.Parameter.PopulationSize).Select(_ => new List<Node>()),
            Regret = Interaction.Parameter.Regret,
            Weight = Interaction.Parameter.Weight,
            Start = start
          };

          var results = Interaction.Algorithm.Search(Interaction.Instance, configuration);
          return results.Sum(nodes => Interaction.Instance.DistanceOf(nodes));
        });
      HandleRunCommand();
    };
    FindWorstButton.Click += (_, _) => {
      ParameterStartIndex.Value = Range((int)ParameterStartIndex.Minimum + 1, (int)ParameterStartIndex.Maximum - 1)
        .MaxBy(start => {
          var configuration = new SearchConfiguration {
            Population = Range(0, Interaction.Parameter.PopulationSize).Select(_ => new List<Node>()),
            Regret = Interaction.Parameter.Regret,
            Start = start
          };

          var results = Interaction.Algorithm.Search(Interaction.Instance, configuration);
          return results.Sum(nodes => Interaction.Instance.DistanceOf(nodes));
        });
      HandleRunCommand();
    };
    CalculateAverageButton.Click += (_, _) => {
      Memory.CalculateAverage((int)ParameterStartIndex.Minimum + 1, (int)ParameterStartIndex.Maximum - 1);
      ChartRefresh();
    };
  }

  private void InitializeComboBoxes() {
    Instances.Items = new List<Option> {
      new("KroA 100", "kroA100"),
      new("KroB 100", "kroB100")
    };
    Instances.SelectedIndex = 0;
    Instances.SelectionChanged += (_, _) => {
      ParameterStartIndex.Maximum = Interaction.Instance.Dimension;
      ParameterPopulationSize.Maximum = Interaction.Hull.Count() - 1;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
      HistorySlider.Value = 0;
      HistorySlider.Maximum = 0;

      Chart.Plot.AutoScale();
      Memory.ClearAverage();
      Memory.Histories.Clear();
    };

    ParameterStartIndex.Maximum = Interaction.Instance.Dimension;
    ParameterPopulationSize.Maximum = Interaction.Hull.Count() - 1;
    ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
    ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);

    Algorithms.SelectionChanged += (_, _) => {
      ParameterRegretBox.IsVisible = Interaction.Algorithm.UsesRegret;
      ParameterWeightBox.IsVisible = Interaction.Algorithm.UsesWeight;
      ParameterRegret.Value = 2;
      Memory.ClearAverage();
      ChartRefresh();
    };

    ParameterStartIndex.ValueChanged += (_, _) => {
      ParameterPopulationSize.Maximum = Interaction.Parameter.StartIndex > Interaction.Hull.Count() ? 2 : Interaction.Hull.Count() - 1;
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
    };
    ParameterPopulationSize.ValueChanged += (_, _) => {
      ParameterStartIndex.Maximum = Interaction.Parameter.PopulationSize > 2 ? Interaction.Hull.Count() - 1 : Interaction.Instance.Dimension;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
    };

    Algorithms.Items = new List<Option> {
      new("Najbliższy sąsiad", Algorithm.NearestNeighbour),
      new("Rozszerzanie cyklu", Algorithm.CycleExpansion),
      new("Rozszerzanie cyklu z k-żalem", Algorithm.CycleExpansionWithKRegret),
      new("Rozszerzanie cyklu z ważonym k-żalem", Algorithm.CycleExpansionWithKRegretAndWeight),
      new("GRASP", Algorithm.RandomAdaptive)
    };
    Algorithms.SelectedIndex = 0;
  }

  private void InitializeChart() {
    Chart.Plot.AutoScale();
    ChartRefresh();

    Chart.PointerMoved += (_, _) => {
      UpdateClosestNode();
      ChartRefresh();
    };
    Chart.PointerReleased += (_, _) => {
      UpdateSelectedNode();
      ChartRefresh();
    };
  }

  private void ChartRefresh() {
    Chart.Plot.Clear();

    Chart.Plot.Add.Scatter(Interaction.Instance.Nodes);

    Memory.Histories.ToList().ForEach(HandleHistoryRender);
    HandleRenderClosestNode();
    HandleRenderSelectedNode();

    HandleRenderAverageCycleDistance();
    HandleRenderDistanceSum();
    HandleUpdateTitle();
    Chart.Refresh();
  }

  private void HandleUpdateTitle() {
    var (mx, my) = Chart.Interaction.GetMouseCoordinates();

    Title = $"Pozycja Myszy - {(int)mx}x, {(int)my}y";
    if (_selectedNode is null) return;
    Title += $" : Wierzchołek - {_selectedNode.Index + 1} - {_selectedNode.X}x, {_selectedNode.Y}y";
    var contained = Memory.Histories.Where(history => history.Count > Interaction.Step)
      .FirstOrDefault(x => x[Interaction.Step].Contains(_selectedNode));
    if (contained is null) return;
    var index = contained[Interaction.Step].IndexOf(_selectedNode);
    Title += $" : Indeks - {index}";
  }

  private void HandleRenderAverageCycleDistance() {
    if (Memory.Average is null) return;
    Chart.Plot.Add.Label($"Przeciętna długość: {Memory.Average:F2}");
  }

  private void HandleRenderDistanceSum() {
    if (Interaction.Parameter.PopulationSize < 2) return;
    Chart.Plot.Add.Label($"Łączna długość: {Memory.Histories.Sum(history =>
      Interaction.Instance.DistanceOf(history.ElementAtOrDefault(Interaction.Step) ?? history[^1]))
    }");
  }

  private void HandleRenderSelectedNode() {
    if (_selectedNode is null) return;
    Chart.Plot.Add.Point(_selectedNode);

    var color = Chart.Plot.Plottables.Count;

    var plotted = new List<Node>();
    foreach (var history in Memory.Histories.Where(history => history.Count > Interaction.Step)) {
      var nodes = history[Interaction.Step].Except(Yield(_selectedNode)).ToList();

      plotted.AddRange(nodes);
      Chart.Plot.Add.DistanceTo(_selectedNode, nodes, Memory._palette.GetColor(++color).ToSKColor());
    }

    Chart.Plot.Add.DistanceTo(_selectedNode, Interaction.Instance.Nodes.Except(plotted).Except(Yield(_selectedNode)));
  }

  private void HandleRenderClosestNode() {
    if (_closestNode is null) return;
    Chart.Plot.Add.Point(_closestNode);
  }

  private void HandleHistoryRender(IReadOnlyList<List<Node>> history) {
    var step = history.ElementAtOrDefault(Interaction.Step) ?? history[^1];

    switch (Interaction.Algorithm.DisplayAs) {
      case Algorithm.DisplayType.Cycle: {
        Chart.Plot.Add.Cycle(step, Interaction.Instance);
        return;
      }
      case Algorithm.DisplayType.Path: {
        if (Interaction.Step == (int)HistorySlider.Maximum) Chart.Plot.Add.Cycle(step, Interaction.Instance);
        else Chart.Plot.Add.Path(step, Interaction.Instance);
        return;
      }
    }
  }

  private void HandleRunCommand() {
    Memory.Histories.Clear();

    var histories = Range(0, Interaction.Parameter.PopulationSize).Select(_ => new List<List<Node>> { new() }).ToList();
    var configuration = Interaction.Parameter.Configuration with {
      Population = histories.Select(history =>
        new ObservableList<Node>(items => history.Add(items.ToList()))
      ),
    };

    Interaction.Algorithm.Search(Interaction.Instance, configuration);

    histories.ForEach(Memory.Histories.Add);
    HistorySlider.Maximum = histories.MaxBy(x => x.Count)!.Count - 1;
    HistorySlider.Value = HistorySlider.Maximum;
  }
}
