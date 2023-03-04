using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Algorithms.Algorithms;
using Algorithms.DataStructures;
using Algorithms.Extensions;
using Algorithms.Methods;
using Avalonia.Controls;
using Avalonia.Media;
using Interface.Types;
using ScottPlot;

namespace Interface;

public partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();
    InitializeComboBoxes();
    InitializeListeners();
    InitializeChart();

    Chart.PointerMoved += (_, _) => { };
  }

  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;
    _histories.CollectionChanged += (_, _) => ChartRefresh();

    HistorySlider.PropertyChanged += (_, _) => {
      HistoryText.Text = $"Krok: {HistoryStep}";
      ChartRefresh();
    };

    StepNextButton.Click += (_, _) => {
      if (HistoryStep > HistorySlider.Maximum) return;
      HistorySlider.Value = HistoryStep + 1;
      ChartRefresh();
    };
    StepBackButton.Click += (_, _) => {
      if (HistoryStep < 0) return;
      HistorySlider.Value = HistoryStep - 1;
      ChartRefresh();
    };
    RunButton.Click += (_, _) => {
      _histories.Clear();

      if (SelectedAlgorithm == Algorithm.GreedyNearestNeighbour) {
        var observed = new ObservableList<Node>();
        var history = new List<List<Node>>();
        observed.Changed += (_, _) => history.Add(observed.ToList());
        Instance.SearchWithGreedyNearestNeighbour(observed);
        _histories.Add(history);
        HistorySlider.Value = Instance.Dimension;
        HistorySlider.Maximum = Instance.Dimension;
      } else if (SelectedAlgorithm == Algorithm.DoubleGreedyNearestNeighbour) {
        var firstObserved = new ObservableList<Node>();
        var secondObserved = new ObservableList<Node>();
        var firstHistory = new List<List<Node>>();
        var secondHistory = new List<List<Node>>();
        firstObserved.Changed += (_, _) => firstHistory.Add(firstObserved.ToList());
        secondObserved.Changed += (_, _) => secondHistory.Add(secondObserved.ToList());
        Instance.SearchDoubleWithGreedyNearestNeighbour(firstObserved, secondObserved);
        _histories.Add(firstHistory);
        _histories.Add(secondHistory);
        HistorySlider.Value = Instance.Dimension / 2;
        HistorySlider.Maximum = Instance.Dimension / 2;
      }
    };
  }

  private void InitializeComboBoxes() {
    Instances.Items = new List<Option> {
      new("KroA 100", "kroA100"),
      new("KroB 100", "kroB100")
    };
    Instances.SelectedIndex = 0;
    Instances.SelectionChanged += (_, _) => {
      Instance = Instance.Read(SelectedInstance);
      HistorySlider.Value = 0;
      HistorySlider.Maximum = 0;

      Chart.Plot.AutoScale();
      _histories.Clear();
    };
    Instance = Instance.Read(SelectedInstance);

    Algorithms.Items = new List<Option> {
      new("Najbliższy sąsiad", Algorithm.GreedyNearestNeighbour),
      new("Podwójny najbliższy sąsiad", Algorithm.DoubleGreedyNearestNeighbour),
      new("Rozszerzanie cyklu", Algorithm.GreedyCycleExpansion),
      new("Rozszerzanie cyklu z 2-żalem", Algorithm.GreedyCycleExpansionWith2Regret),
    };
    Algorithms.SelectedIndex = 0;
  }

  private void InitializeChart() {
    Chart.Plot.AutoScale();
    ChartRefresh();
  }

  private void ChartRefresh() {
    Chart.Plot.Clear();
    Chart.Plot.Add.Scatter(Instance.Nodes, Instance);
    foreach (var history in _histories.Where(x => HistoryStep > 0)) {
      if (HistoryStep == (int)HistorySlider.Maximum) Chart.Plot.Add.Cycle(history.Last(), Instance);
      else Chart.Plot.Add.Path(history[HistoryStep - 1], Instance);
    }
    Chart.Refresh();
  }

  private Instance Instance = null!;
  private string SelectedInstance => Instances.SelectedItem.As<Option>().Value;
  private string SelectedAlgorithm => Algorithms.SelectedItem.As<Option>().Value;
  private int HistoryStep => (int)HistorySlider.Value;
  private readonly ObservableCollection<List<List<Node>>> _histories = new();

}
