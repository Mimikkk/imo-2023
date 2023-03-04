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
  private readonly Pixel HighlightedPoint = new();
  private int LastHighlightedIndex = -1;
  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;
    _histories.CollectionChanged += (_, _) => ChartRefresh();

    HistorySlider.PropertyChanged += (_, _) => {
      HistoryText.Text = $"Krok: {HistoryStep}";
      ChartRefresh();
    };

    StepNextButton.Click += (_, _) => {
      if (HistoryStep > Instance.Dimension) return;
      HistorySlider.Value = HistoryStep + 1;
      ChartRefresh();
    };
    StepBackButton.Click += (_, _) => {
      if (HistoryStep < 0) return;
      HistorySlider.Value = HistoryStep - 1;
      ChartRefresh();
    };
    RunButton.Click += (_, _) => {
      var observed = new ObservableList<Node>();

      _histories.Clear();
      var first = new List<List<Node>>();
      observed.Changed += (_, _) => first.Add(observed.ToList());

      Debug.WriteLine(first.Count);
      Instance.SearchWithGreedyNearestNeighbour(observed);
      Debug.WriteLine(first.Count);
      HistorySlider.Value = Instance.Dimension;
      HistorySlider.Maximum = Instance.Dimension;

      _histories.Add(first);
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
      new("Najbliższy sąsiad", "greedy-nearest-neighbor"),
      new("Rozszerzanie cyklu", "greedy-cycle-expansion"),
      new("Rozszerzanie cyklu z 2-żalem", "greedy-cycle-expansion-with-2-regret"),
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
      if (HistoryStep == Instance.Dimension) Chart.Plot.Add.Cycle(history.Last(), Instance);
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
