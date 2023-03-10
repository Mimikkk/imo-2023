using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Algorithms.Algorithms;
using Algorithms.DataStructures;
using Algorithms.Extensions;
using Algorithms.Methods;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Interface.Types;
using ScottPlot;
using ScottPlot.Control;
using ScottPlot.Palettes;
using static System.Linq.Enumerable;
using static Algorithms.Extensions.IEnumerableExtensions;

namespace Interface;

public partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();
    InitializeComboBoxes();
    InitializeChart();
    InitializeListeners();
  }

  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;


    _histories.CollectionChanged += (_, _) => ChartRefresh();

    HistorySlider.PropertyChanged += (_, change) => {
      if (change.Property.Name != "Value") return;

      HistoryText.Text = $"Krok: {HistoryStep}";
      ChartRefresh();
    };
    StepBackButton.Click += (_, _) => HistorySlider.Value = HistoryStep - 1;
    StepNextButton.Click += (_, _) => HistorySlider.Value = HistoryStep + 1;
    RunButton.Click += (_, _) => HandleRunCommand();

    ClearParameterStartIndexButton.Click += (_, _) => ParameterStartIndex.Value = 0;
    ParameterStartIndex.Value = 0;
    ClearParameterRegretButton.Click += (_, _) => ParameterRegret.Value = 2;
    ParameterRegret.Value = 2;
    ClearParameterPopulationSizeButton.Click += (_, _) => ParameterPopulationSize.Value = 1;
    ParameterPopulationSize.Value = 1;
  }

  private void InitializeComboBoxes() {
    Instances.Items = new List<Option> {
      new("KroA 100", "kroA100"),
      new("KroB 100", "kroB100")
    };
    Instances.SelectedIndex = 0;
    Instances.SelectionChanged += (_, _) => {
      _instance = Instance.Read(SelectedInstance);
      _instanceHull = _instance.Nodes.Hull().ToList();
      ParameterStartIndex.Maximum = _instance.Dimension;
      ParameterPopulationSize.Maximum = _instanceHull.Count - 1;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
      HistorySlider.Value = 0;
      HistorySlider.Maximum = 0;

      Chart.Plot.AutoScale();
      _histories.Clear();
    };
    _instance = Instance.Read(SelectedInstance);
    _instanceHull = _instance.Nodes.Hull().ToList();
    ParameterStartIndex.Maximum = _instance.Dimension;
    ParameterPopulationSize.Maximum = _instanceHull.Count - 1;
    ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
    ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);

    Algorithms.SelectionChanged += (_, _) => {
      ParameterRegretBox.IsVisible = SelectedAlgorithm == Algorithm.NGreedyCycleExpansionWithKRegret;
      ParameterRegret.Value = 2;
    };

    ParameterStartIndex.ValueChanged += (_, _) => {
      ParameterPopulationSize.Maximum = SelectedParameterStartIndex > _instanceHull.Count ? 2 : _instanceHull.Count - 1;
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
    };
    ParameterPopulationSize.ValueChanged += (_, _) => {
      ParameterStartIndex.Maximum = SelectedParameterPopulationSize > 2 ? _instanceHull.Count - 1 : _instance.Dimension;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
    };

    Algorithms.Items = new List<Option> {
      new("Wieloraki Najbliższy sąsiad", Algorithm.NGreedyNearestNeighbour),
      new("Wielorakie Rozszerzanie cyklu", Algorithm.NGreedyCycleExpansion),
      new("Wielorakie Rozszerzanie cyklu z k-żalem", Algorithm.NGreedyCycleExpansionWithKRegret),
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

    Chart.Plot.Add.Scatter(_instance.Nodes);

    _histories.ToList().ForEach(HandleHistoryRender);
    HandleRenderClosestNode();
    HandleRenderSelectedNode();

    HandleUpdateTitle();
    Chart.Refresh();
  }

  private void HandleUpdateTitle() {
    var (mx, my) = Chart.Interaction.GetMouseCoordinates();

    Title = $"Pozycja Myszy - {(int)mx}x, {(int)my}y";
    if (_selectedNode is null) return;
    Title += $" : Wierzchołek - {_selectedNode.Index + 1} - {_selectedNode.X}x, {_selectedNode.Y}y";
    var contained = _histories.Where(history => history.Count > HistoryStep)
      .FirstOrDefault(x => x[HistoryStep].Contains(_selectedNode));
    if (contained is null) return;
    var index = contained[HistoryStep].IndexOf(_selectedNode);
    Title += $" : Indeks - {index}";
  }

  private void HandleRenderSelectedNode() {
    if (_selectedNode is null) return;
    Chart.Plot.Add.Point(_selectedNode);

    var color = Chart.Plot.Plottables.Count;

    var plotted = new List<Node>();
    foreach (var history in _histories.Where(history => history.Count > HistoryStep)) {
      var nodes = history[HistoryStep].Except(Yield(_selectedNode)).ToList();

      plotted.AddRange(nodes);
      Chart.Plot.Add.DistanceTo(_selectedNode, nodes, _palette.GetColor(++color).ToSKColor());
    }

    Chart.Plot.Add.DistanceTo(_selectedNode, _instance.Nodes.Except(plotted).Except(Yield(_selectedNode)));
  }

  private void HandleRenderClosestNode() {
    if (_closestNode is null) return;
    Chart.Plot.Add.Point(_closestNode);
  }

  private void HandleHistoryRender(IReadOnlyList<List<Node>> history) {
    var step = history.ElementAtOrDefault(HistoryStep) ?? history[^1];

    switch (SelectedAlgorithm.Type) {
      case Algorithm.StrategyType.CycleBased: {
        Chart.Plot.Add.Cycle(step, _instance);
        return;
      }
      case Algorithm.StrategyType.PathBased: {
        if (HistoryStep == (int)HistorySlider.Maximum) Chart.Plot.Add.Cycle(step, _instance);
        else Chart.Plot.Add.Path(step, _instance);
        return;
      }
    }
  }

  private void HandleRunCommand() {
    _histories.Clear();

    var histories = Range(0, SelectedParameterPopulationSize).Select(_ => new List<List<Node>> { new() }).ToList();
    var configuration = new SearchConfiguration(
      histories.Select(history =>
        new ObservableList<Node>(items => history.Add(items.ToList()))
      ),
      SelectedParameterRegret,
      SelectedParameterStartIndex
    );
    var x = SelectedAlgorithm.Search(_instance, configuration);

    histories.ForEach(_histories.Add);
    HistorySlider.Maximum = histories.MaxBy(x => x.Count)!.Count - 1;
    HistorySlider.Value = HistorySlider.Maximum;
  }

  private IList<Node> _instanceHull = null!;
  private Instance _instance = null!;
  private string SelectedInstance => Instances.SelectedItem.As<Option>().Value;
  private Algorithm SelectedAlgorithm => Algorithm.FromName(Algorithms.SelectedItem.As<Option>().Value);
  private int HistoryStep => (int)HistorySlider.Value;
  private readonly ObservableCollection<List<List<Node>>> _histories = new();
  private readonly IPalette _palette = new Category10();
}
