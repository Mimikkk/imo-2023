using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Structures;
using Avalonia.Controls;
using Domain.Structures;
using Interface.Types;
using static System.Linq.Enumerable;

namespace Interface;

public sealed partial class MainWindow : Window {
  public MainWindow() {
    Mod = new(this);

    Mod.Chart.Subscribe(Mod.Title.Update);

    InitializeComponent();
    InitializeComboBoxes();
    InitializeChart();
    InitializeListeners();
  }

  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;


    Mod.Memory.Histories.CollectionChanged += (_, _) => Mod.Chart.Notify();

    HistorySlider.PropertyChanged += (_, change) => {
      if (change.Property.Name != "Value") return;

      HistoryText.Text = $"Krok: {Mod.Interaction.Step}";
      Mod.Chart.Notify();
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
            Population = Range(0, Mod.Interaction.Parameter.PopulationSize).Select(_ => new List<Node>()),
            Regret = Mod.Interaction.Parameter.Regret,
            Weight = Mod.Interaction.Parameter.Weight,
            Start = start
          };

          var results = Mod.Interaction.Algorithm.Search(Mod.Interaction.Instance, configuration);
          return results.Sum(nodes => Mod.Interaction.Instance.DistanceOf(nodes));
        });
      HandleRunCommand();
    };
    FindWorstButton.Click += (_, _) => {
      ParameterStartIndex.Value = Range((int)ParameterStartIndex.Minimum + 1, (int)ParameterStartIndex.Maximum - 1)
        .MaxBy(start => {
          var configuration = new SearchConfiguration {
            Population = Range(0, Mod.Interaction.Parameter.PopulationSize).Select(_ => new List<Node>()),
            Regret = Mod.Interaction.Parameter.Regret,
            Start = start
          };

          var results = Mod.Interaction.Algorithm.Search(Mod.Interaction.Instance, configuration);
          return results.Sum(nodes => Mod.Interaction.Instance.DistanceOf(nodes));
        });
      HandleRunCommand();
    };
    CalculateAverageButton.Click += (_, _) => {
      Mod.Memory.CalculateAverage((int)ParameterStartIndex.Minimum + 1, (int)ParameterStartIndex.Maximum - 1);
      Mod.Chart.Notify();
    };
  }

  private void InitializeComboBoxes() {
    Instances.Items = new List<Option> {
      new("KroA 100", "kroA100"),
      new("KroB 100", "kroB100")
    };
    Instances.SelectedIndex = 0;
    Instances.SelectionChanged += (_, _) => {
      ParameterStartIndex.Maximum = Mod.Interaction.Instance.Dimension;
      ParameterPopulationSize.Maximum = Mod.Interaction.Hull.Count() - 1;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
      HistorySlider.Value = 0;
      HistorySlider.Maximum = 0;

      Chart.Plot.AutoScale();
      Mod.Memory.ClearAverage();
      Mod.Memory.Histories.Clear();
    };

    ParameterStartIndex.Maximum = Mod.Interaction.Instance.Dimension;
    ParameterPopulationSize.Maximum = Mod.Interaction.Hull.Count() - 1;
    ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
    ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);

    Algorithms.SelectionChanged += (_, _) => {
      ParameterRegretBox.IsVisible = Mod.Interaction.Algorithm.UsesRegret;
      ParameterWeightBox.IsVisible = Mod.Interaction.Algorithm.UsesWeight;
      ParameterRegret.Value = 2;
      Mod.Memory.ClearAverage();
      Mod.Chart.Notify();
    };

    ParameterStartIndex.ValueChanged += (_, _) => {
      ParameterPopulationSize.Maximum = Mod.Interaction.Parameter.StartIndex > Mod.Interaction.Hull.Count() ? 2 : Mod.Interaction.Hull.Count() - 1;
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
    };
    ParameterPopulationSize.ValueChanged += (_, _) => {
      ParameterStartIndex.Maximum = Mod.Interaction.Parameter.PopulationSize > 2 ? Mod.Interaction.Hull.Count() - 1 : Mod.Interaction.Instance.Dimension;
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
    Mod.Chart.Notify();

    Chart.PointerMoved += (_, _) => {
      UpdateClosestNode();
      Mod.Chart.Notify();
    };
    Chart.PointerReleased += (_, _) => {
      UpdateSelectedNode();
      Mod.Chart.Notify();
    };
  }

  private void HandleRunCommand() {
    Mod.Memory.Histories.Clear();

    var histories = Range(0, Mod.Interaction.Parameter.PopulationSize).Select(_ => new List<List<Node>> { new() }).ToList();
    Mod.Interaction.Algorithm.Search(Mod.Interaction.Instance, Mod.Interaction.Parameter.Configuration with {
      Population = histories.Select(history =>
        new ObservableList<Node>(items => history.Add(items.ToList()))
      ),
    });

    histories.ForEach(Mod.Memory.Histories.Add);
    HistorySlider.Maximum = histories.MaxBy(x => x.Count)!.Count - 1;
    HistorySlider.Value = HistorySlider.Maximum;
  }
  
  internal readonly Modules.Modules Mod;
}
