using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using static Algorithms.Extensions.IEnumerableExtensions;

namespace Interface;

public partial class MainWindow : Window {
  private Node FindClosestNodeToMouse() {
    var mouse = Chart.Interaction.GetMouseCoordinates();
    var closest = Instance.Nodes.MinBy(node => node.DistanceTo(mouse))!;
    return closest;
  }

  public MainWindow() {
    InitializeComponent();
    InitializeComboBoxes();
    InitializeChart();
    InitializeListeners();
  }

  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;

    Histories.CollectionChanged += (_, _) => ChartRefresh();

    HistorySlider.PropertyChanged += (_, change) => {
      if (change.Property.Name != "Value") return;

      HistoryText.Text = $"Krok: {HistoryStep}";
      ChartRefresh();
    };
    StepBackButton.Click += (_, _) => HistorySlider.Value = HistoryStep - 1;
    StepNextButton.Click += (_, _) => HistorySlider.Value = HistoryStep + 1;
    RunButton.Click += (_, _) => {
      Histories.Clear();
      if (SelectedAlgorithm == Algorithm.GreedyNearestNeighbour) {
        HistorySlider.Maximum = Instance.Dimension;
        HistorySlider.Value = Instance.Dimension;

        var observed = new ObservableList<Node>();
        var history = new List<List<Node>> { new() };
        observed.Changed += (_, _) => history.Add(observed.ToList());
        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        Instance.SearchWithGreedyNearestNeighbour(observed, startIndex);
        history.Add(observed.ToList());

        Histories.Add(history);
      } else if (SelectedAlgorithm == Algorithm.DoubleGreedyNearestNeighbour) {
        HistorySlider.Maximum = Instance.Dimension / 2;
        HistorySlider.Value = Instance.Dimension / 2;
        var firstObserved = new ObservableList<Node>();
        var secondObserved = new ObservableList<Node>();
        var firstHistory = new List<List<Node>> { new() };
        var secondHistory = new List<List<Node>> { new() };
        firstObserved.Changed += (_, _) => firstHistory.Add(firstObserved.ToList());
        secondObserved.Changed += (_, _) => secondHistory.Add(secondObserved.ToList());
        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        Instance.SearchWithGreedyNearestNeighbour(firstObserved, secondObserved, startIndex);
        firstHistory.Add(firstObserved.ToList());
        secondHistory.Add(secondObserved.ToList());
        Histories.Add(firstHistory);
        Histories.Add(secondHistory);
      } else if (SelectedAlgorithm == Algorithm.GreedyCycleExpansion) {
        HistorySlider.Maximum = Instance.Dimension;
        HistorySlider.Value = Instance.Dimension;
        var observed = new ObservableList<Node>();
        var history = new List<List<Node>> { new() };
        observed.Changed += (_, _) => history.Add(observed.ToList());

        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        Instance.SearchWithGreedyCycleExpansion(observed, startIndex);
        history.Add(observed.ToList());
        Histories.Add(history);
      } else if (SelectedAlgorithm == Algorithm.DoubleGreedyCycleExpansion) {
        HistorySlider.Maximum = Instance.Dimension / 2;
        HistorySlider.Value = Instance.Dimension / 2;
        var firstObserved = new ObservableList<Node>();
        var secondObserved = new ObservableList<Node>();
        var firstHistory = new List<List<Node>> { new() };
        var secondHistory = new List<List<Node>> { new() };
        firstObserved.Changed += (_, _) => firstHistory.Add(firstObserved.ToList());
        secondObserved.Changed += (_, _) => secondHistory.Add(secondObserved.ToList());
        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        Instance.SearchWithGreedyCycleExpansion(firstObserved, secondObserved, startIndex);
        firstHistory.Add(firstObserved.ToList());
        secondHistory.Add(secondObserved.ToList());
        Histories.Add(firstHistory);
        Histories.Add(secondHistory);
      } else if (SelectedAlgorithm == Algorithm.GreedyCycleExpansionWith2Regret) {
        HistorySlider.Maximum = Instance.Dimension;
        HistorySlider.Value = Instance.Dimension;
        var observed = new ObservableList<Node>();
        var history = new List<List<Node>> { new() };
        observed.Changed += (_, _) => history.Add(observed.ToList());

        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        Instance.SearchWithGreedyCycleExpansionWith2Regret(observed, startIndex);
        history.Add(observed.ToList());
        Histories.Add(history);
      } else if (SelectedAlgorithm == Algorithm.DoubleGreedyCycleExpansionWith2Regret) {
        HistorySlider.Maximum = Instance.Dimension / 2;
        HistorySlider.Value = Instance.Dimension / 2;
        var firstObserved = new ObservableList<Node>();
        var secondObserved = new ObservableList<Node>();
        var firstHistory = new List<List<Node>> { new() };
        var secondHistory = new List<List<Node>> { new() };
        firstObserved.Changed += (_, _) => firstHistory.Add(firstObserved.ToList());
        secondObserved.Changed += (_, _) => secondHistory.Add(secondObserved.ToList());
        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        Instance.SearchWithGreedyCycleExpansionWith2Regret(firstObserved, secondObserved, startIndex);
        firstHistory.Add(firstObserved.ToList());
        secondHistory.Add(secondObserved.ToList());
        Histories.Add(firstHistory);
        Histories.Add(secondHistory);
      }
    };
    ClearStartIndexButton.Click += (_, _) => StartIndex.Value = 0;
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
      StartIndex.Maximum = Instance.Dimension;

      Chart.Plot.AutoScale();
      Histories.Clear();
    };
    Instance = Instance.Read(SelectedInstance);
    StartIndex.Value = 53;
    StartIndex.Minimum = 0;
    StartIndex.Maximum = Instance.Dimension;

    Algorithms.Items = new List<Option> {
      new("Najbliższy sąsiad", Algorithm.GreedyNearestNeighbour),
      new("Podwójny najbliższy sąsiad", Algorithm.DoubleGreedyNearestNeighbour),
      new("Rozszerzanie cyklu", Algorithm.GreedyCycleExpansion),
      new("Podwójne Rozszerzanie cyklu", Algorithm.DoubleGreedyCycleExpansion),
      new("Rozszerzanie cyklu z 2-żalem", Algorithm.GreedyCycleExpansionWith2Regret),
      new("Podwójne Rozszerzanie cyklu z 2-żalem", Algorithm.DoubleGreedyCycleExpansionWith2Regret),
    };
    Algorithms.SelectedIndex = 0;
  }

  private void InitializeChart() {
    Chart.Plot.AutoScale();
    ChartRefresh();

    Chart.PointerMoved += (_, _) => {
      ClosestNode = FindClosestNodeToMouse();
      ChartRefresh();
    };
    Chart.PointerReleased += (_, _) => {
      var mouse = Chart.Interaction.GetMouseCoordinates();
      if (mouse.DistanceTo(ClosestNode!) < 125)
        SelectedNode = SelectedNode == ClosestNode ? null : ClosestNode;
      ChartRefresh();
    };
  }

  private void ChartRefresh() {
    Chart.Plot.Clear();
    Chart.Plot.Add.Scatter(Instance.Nodes, Instance);

    foreach (var history in Histories) {
      if (
        SelectedAlgorithm == Algorithm.GreedyCycleExpansion
        || SelectedAlgorithm == Algorithm.DoubleGreedyCycleExpansion
        || SelectedAlgorithm == Algorithm.GreedyCycleExpansionWith2Regret
        || SelectedAlgorithm == Algorithm.DoubleGreedyCycleExpansionWith2Regret) {
        Chart.Plot.Add.Cycle(history[HistoryStep], Instance);
      } else {
        if (HistoryStep == (int)HistorySlider.Maximum) Chart.Plot.Add.Cycle(history[HistoryStep], Instance);
        else Chart.Plot.Add.Path(history[HistoryStep], Instance);
      }
    }

    if (ClosestNode is not null) Chart.Plot.Add.Point(ClosestNode);
    if (SelectedNode is not null) {
      Chart.Plot.Add.Point(SelectedNode);

      var color = Chart.Plot.Plottables.Count;

      var plotted = new List<Node>();
      foreach (var history in Histories) {
        var nodes = history[HistoryStep].Except(Yield(SelectedNode)).ToList();

        plotted.AddRange(nodes);
        Chart.Plot.Add.DistanceTo(SelectedNode, nodes, Palette.GetColor(++color).ToSKColor());
      }
      Chart.Plot.Add.DistanceTo(SelectedNode, Instance.Nodes.Except(plotted).Except(Yield(SelectedNode)));
    }

    var (mx, my) = Chart.Interaction.GetMouseCoordinates();

    Title = $"Pozycja Myszy - {(int)mx}x, {(int)my}y";
    if (SelectedNode is not null) {
      Title += $" : Wierzchołek - {SelectedNode.Index + 1} - {SelectedNode.X}x, {SelectedNode.Y}y";
    }
    if (Histories.Count > 0 && SelectedNode is not null) {
      var contained = Histories.FirstOrDefault(x => x[HistoryStep].Contains(SelectedNode));
      if (contained is not null) {
        var index = contained[HistoryStep].IndexOf(SelectedNode);
        Title += $" : Indeks - {index}";
      }
    }

    Chart.Refresh();
  }

  private Node? ClosestNode;
  private Node? SelectedNode;
  private Instance Instance = null!;
  private string SelectedInstance => Instances.SelectedItem.As<Option>().Value;
  private string SelectedAlgorithm => Algorithms.SelectedItem.As<Option>().Value;
  private int HistoryStep => (int)HistorySlider.Value;
  private readonly ObservableCollection<List<List<Node>>> Histories = new();
  private readonly IPalette Palette = new Category10();
}
