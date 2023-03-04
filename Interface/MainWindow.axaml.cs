using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Algorithms.Algorithms;
using Algorithms.DataStructures;
using Algorithms.Extensions;
using Algorithms.Methods;
using Avalonia.Controls;
using Interface.Types;
using ScottPlot;

namespace Interface;

public partial class MainWindow : Window {

  private string SelectedInstance => Instances.SelectedItem.As<Option>().Value;
  private string SelectedAlgorithm => Algorithms.SelectedItem.As<Option>().Value;
  private int CurrentHistoryStep => (int)HistorySlider.Value;
  private ObservableCollection<List<Node>> History { get; set; } = new();
  private Instance? CurrentInstance;
  public MainWindow() {
    InitializeComponent();
    RunStartup();
  }

  private void InitializeListeners() {
    History.CollectionChanged += (_, _) => {
      HistorySlider.Maximum = History.Count;
      HistorySlider.Value = History.Count;
    };

    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;
    HistorySlider.Maximum = 0;
    HistorySlider.PropertyChanged += (_, _) => {
      if (CurrentInstance is null) return;
      HistoryText.Text = $"Krok: {CurrentHistoryStep}";

      Chart.Plot.Clear();
      Chart.Plot.Add.Scatter(CurrentInstance.Nodes, CurrentInstance);
      if (History.ElementAtOrDefault(CurrentHistoryStep - 1) is { } nodes) {
        if (nodes == History.Last()) Chart.Plot.Add.Cycle(nodes, CurrentInstance);
        else Chart.Plot.Add.Path(nodes, CurrentInstance);
      }
      Chart.Refresh();
    };

    StepNextButton.Click += (_, _) => {
      if (CurrentHistoryStep >= History.Count - 1) return;
      HistorySlider.Value = CurrentHistoryStep + 1;
    };
    StepBackButton.Click += (_, _) => {
      if (CurrentHistoryStep <= 0) return;
      HistorySlider.Value = CurrentHistoryStep - 1;
    };
    RunButton.Click += (_, _) => {
      if (CurrentInstance is null) return;
      var observed = new ObservableList<Node>();
      History.Clear();
      observed.Changed += (_, _) => History.Add(observed.ToList());
      CurrentInstance.SearchWithGreedyNearestNeighbour(observed);

      Chart.Plot.SetAxisLimits(
        0,
        CurrentInstance.Nodes.Max(x => x.X),
        0,
        CurrentInstance.Nodes.Max(x => x.Y)
      );
    };
  }

  private void InitializeComboBoxes() {
    Instances.Items = new List<Option> {
      new("KroA 100", "kroA100"),
      new("KroB 100", "kroB100")
    };
    Instances.SelectedIndex = 0;
    Instances.PropertyChanged += (_, _) => CurrentInstance = Instance.Read(SelectedInstance);

    Algorithms.Items = new List<Option> {
      new("Najbliższy sąsiad", "greedy-nearest-neighbor"),
      new("Rozszerzanie cyklu", "greedy-cycle-expansion"),
      new("Rozszerzanie cyklu z 2-żalem", "greedy-cycle-expansion-with-2-regret"),
    };
    Algorithms.SelectedIndex = 0;
  }

  private void InitializeChart() { }

  private void RunStartup() {
    GetType()
      .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
      .Where(x => x.Name.StartsWith("Initialize"))
      .ToList()
      .ForEach(x => x.Invoke(this, null));
  }
}
