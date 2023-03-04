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

namespace Interface;

public partial class MainWindow : Window {

  private string SelectedInstance => Instances.SelectedItem.As<Option>().Value;
  private string SelectedAlgorithm => Algorithms.SelectedItem.As<Option>().Value;
  private int CurrentHistoryStep => (int)HistorySlider.Value;
  private readonly ObservableCollection<List<Node>> _history = new();
  private Instance CurrentInstance = null!;
  public MainWindow() {
    InitializeComponent();
    InitializeComboBoxes();
    InitializeListeners();
    InitializeChart();
  }

  private void InitializeListeners() {
    _history.CollectionChanged += (_, _) => {
      HistorySlider.Maximum = _history.Count;
      HistorySlider.Value = _history.Count;
    };

    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;
    HistorySlider.Maximum = 0;
    HistorySlider.PropertyChanged += (_, _) => {
      HistoryText.Text = $"Krok: {CurrentHistoryStep}";
      ChartRefresh();
    };

    StepNextButton.Click += (_, _) => {
      if (CurrentHistoryStep >= _history.Count - 1) return;
      HistorySlider.Value = CurrentHistoryStep + 1;
    };
    StepBackButton.Click += (_, _) => {
      if (CurrentHistoryStep <= 0) return;
      HistorySlider.Value = CurrentHistoryStep - 1;
    };
    RunButton.Click += (_, _) => {
      var observed = new ObservableList<Node>();
      _history.Clear();
      observed.Changed += (_, _) => _history.Add(observed.ToList());
      CurrentInstance.SearchWithGreedyNearestNeighbour(observed);
    };
  }

  private void InitializeComboBoxes() {
    Instances.Items = new List<Option> {
      new("KroA 100", "kroA100"),
      new("KroB 100", "kroB100")
    };
    Instances.SelectedIndex = 0;
    Instances.SelectionChanged += (_, _) => {
      CurrentInstance = Instance.Read(SelectedInstance);
      _history.Clear();
      ChartZoomIn();
      ChartRefresh();
    };
    CurrentInstance = Instance.Read(SelectedInstance);

    Algorithms.Items = new List<Option> {
      new("Najbliższy sąsiad", "greedy-nearest-neighbor"),
      new("Rozszerzanie cyklu", "greedy-cycle-expansion"),
      new("Rozszerzanie cyklu z 2-żalem", "greedy-cycle-expansion-with-2-regret"),
    };
    Algorithms.SelectedIndex = 0;
  }

  private void InitializeChart() {
    ChartZoomIn();
    ChartRefresh();
  }

  private void ChartRefresh() {
    Chart.Plot.Clear();
    Chart.Plot.Add.Scatter(CurrentInstance.Nodes, CurrentInstance);
    if (_history.ElementAtOrDefault(CurrentHistoryStep - 1) is { } nodes) {
      if (nodes == _history.Last()) Chart.Plot.Add.Cycle(nodes, CurrentInstance);
      else Chart.Plot.Add.Path(nodes, CurrentInstance);
    }
    Chart.Refresh();
  }
  private void ChartZoomIn() {
    Chart.Plot.SetAxisLimits(
      0,
      CurrentInstance.Nodes.Max(x => x.X),
      0,
      CurrentInstance.Nodes.Max(x => x.Y)
    );
  }
}
