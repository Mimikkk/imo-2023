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
    var closest = _instance.Nodes.MinBy(node => node.DistanceTo(mouse))!;
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

    _histories.CollectionChanged += (_, _) => ChartRefresh();

    HistorySlider.PropertyChanged += (_, change) => {
      if (change.Property.Name != "Value") return;

      HistoryText.Text = $"Krok: {HistoryStep}";
      ChartRefresh();
    };
    StepBackButton.Click += (_, _) => HistorySlider.Value = HistoryStep - 1;
    StepNextButton.Click += (_, _) => HistorySlider.Value = HistoryStep + 1;
    RunButton.Click += (_, _) => {
      _histories.Clear();

      if (SelectedAlgorithm == Algorithm.GreedyNearestNeighbour) {
        HistorySlider.Maximum = _instance.Dimension;
        HistorySlider.Value = _instance.Dimension;

        var observed = new ObservableList<Node>();
        var history = new List<List<Node>> { new() };
        observed.Changed += (_, _) => history.Add(observed.ToList());
        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        _instance.SearchWithGreedyNearestNeighbour(observed, startIndex);
        history.Add(observed.ToList());

        _histories.Add(history);
      }
      else if (SelectedAlgorithm == Algorithm.DoubleGreedyNearestNeighbour) {
        HistorySlider.Maximum = _instance.Dimension / 2;
        HistorySlider.Value = _instance.Dimension / 2;
        var firstObserved = new ObservableList<Node>();
        var secondObserved = new ObservableList<Node>();
        var firstHistory = new List<List<Node>> { new() };
        var secondHistory = new List<List<Node>> { new() };
        firstObserved.Changed += (_, _) => firstHistory.Add(firstObserved.ToList());
        secondObserved.Changed += (_, _) => secondHistory.Add(secondObserved.ToList());
        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        _instance.SearchWithGreedyNearestNeighbour(firstObserved, secondObserved, startIndex);
        firstHistory.Add(firstObserved.ToList());
        secondHistory.Add(secondObserved.ToList());
        _histories.Add(firstHistory);
        _histories.Add(secondHistory);
      }
      else if (SelectedAlgorithm == Algorithm.GreedyCycleExpansion) {
        HistorySlider.Maximum = _instance.Dimension;
        HistorySlider.Value = _instance.Dimension;
        var observed = new ObservableList<Node>();
        var history = new List<List<Node>> { new() };
        observed.Changed += (_, _) => history.Add(observed.ToList());

        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        _instance.SearchWithGreedyCycleExpansion(observed, startIndex);
        history.Add(observed.ToList());
        _histories.Add(history);
      }
      else if (SelectedAlgorithm == Algorithm.DoubleGreedyCycleExpansion) {
        HistorySlider.Maximum = _instance.Dimension / 2;
        HistorySlider.Value = _instance.Dimension / 2;
        var firstObserved = new ObservableList<Node>();
        var secondObserved = new ObservableList<Node>();
        var firstHistory = new List<List<Node>> { new() };
        var secondHistory = new List<List<Node>> { new() };
        firstObserved.Changed += (_, _) => firstHistory.Add(firstObserved.ToList());
        secondObserved.Changed += (_, _) => secondHistory.Add(secondObserved.ToList());
        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        _instance.SearchWithGreedyCycleExpansion(firstObserved, secondObserved, startIndex);
        firstHistory.Add(firstObserved.ToList());
        secondHistory.Add(secondObserved.ToList());
        _histories.Add(firstHistory);
        _histories.Add(secondHistory);
      }
      else if (SelectedAlgorithm == Algorithm.GreedyCycleExpansionWith2Regret) {
        HistorySlider.Maximum = _instance.Dimension;
        HistorySlider.Value = _instance.Dimension;
        var observed = new ObservableList<Node>();
        var history = new List<List<Node>> { new() };
        observed.Changed += (_, _) => history.Add(observed.ToList());

        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        _instance.SearchWithGreedyCycleExpansionWith2Regret(observed, startIndex);
        history.Add(observed.ToList());
        _histories.Add(history);
      }
      else if (SelectedAlgorithm == Algorithm.DoubleGreedyCycleExpansionWith2Regret) {
        HistorySlider.Maximum = _instance.Dimension / 2;
        HistorySlider.Value = _instance.Dimension / 2;
        var firstObserved = new ObservableList<Node>();
        var secondObserved = new ObservableList<Node>();
        var firstHistory = new List<List<Node>> { new() };
        var secondHistory = new List<List<Node>> { new() };
        firstObserved.Changed += (_, _) => firstHistory.Add(firstObserved.ToList());
        secondObserved.Changed += (_, _) => secondHistory.Add(secondObserved.ToList());
        int? startIndex = (int)StartIndex.Value == 0 ? null : (int)StartIndex.Value - 1;
        _instance.SearchWithGreedyCycleExpansionWith2Regret(firstObserved, secondObserved, startIndex);
        firstHistory.Add(firstObserved.ToList());
        secondHistory.Add(secondObserved.ToList());
        _histories.Add(firstHistory);
        _histories.Add(secondHistory);
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
      _instance = Instance.Read(SelectedInstance);
      HistorySlider.Value = 0;
      HistorySlider.Maximum = 0;
      StartIndex.Maximum = _instance.Dimension;

      Chart.Plot.AutoScale();
      _histories.Clear();
    };
    _instance = Instance.Read(SelectedInstance);
    StartIndex.Value = 53;
    StartIndex.Minimum = 0;
    StartIndex.Maximum = _instance.Dimension;

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
      _closestNode = FindClosestNodeToMouse();
      ChartRefresh();
    };
    Chart.PointerReleased += (_, _) => {
      var mouse = Chart.Interaction.GetMouseCoordinates();
      if (mouse.DistanceTo(_closestNode!) < 125)
        _selectedNode = _selectedNode == _closestNode ? null : _closestNode;
      ChartRefresh();
    };
  }

  private void ChartRefresh() {
    Chart.Plot.Clear();
    Chart.Plot.Add.Scatter(_instance.Nodes, _instance);
    Chart.Plot.Add.Cycle(_instance.Nodes.Hull(), _instance);
    var farthest = _instance.ChooseFurthest(5);
    Chart.Plot.Add.Scatter(farthest, _instance);

    foreach (var history in _histories) {
      if (
        SelectedAlgorithm == Algorithm.GreedyCycleExpansion
        || SelectedAlgorithm == Algorithm.DoubleGreedyCycleExpansion
        || SelectedAlgorithm == Algorithm.GreedyCycleExpansionWith2Regret
        || SelectedAlgorithm == Algorithm.DoubleGreedyCycleExpansionWith2Regret) {
        Chart.Plot.Add.Cycle(history[HistoryStep], _instance);
      }
      else {
        if (HistoryStep == (int)HistorySlider.Maximum) Chart.Plot.Add.Cycle(history[HistoryStep], _instance);
        else Chart.Plot.Add.Path(history[HistoryStep], _instance);
      }
    }

    if (_closestNode is not null) Chart.Plot.Add.Point(_closestNode);
    if (_selectedNode is not null) {
      Chart.Plot.Add.Point(_selectedNode);

      var color = Chart.Plot.Plottables.Count;

      var plotted = new List<Node>();
      foreach (var history in _histories) {
        var nodes = history[HistoryStep].Except(Yield(_selectedNode)).ToList();

        plotted.AddRange(nodes);
        Chart.Plot.Add.DistanceTo(_selectedNode, nodes, _palette.GetColor(++color).ToSKColor());
      }

      Chart.Plot.Add.DistanceTo(_selectedNode, _instance.Nodes.Except(plotted).Except(Yield(_selectedNode)));
    }

    var (mx, my) = Chart.Interaction.GetMouseCoordinates();

    Title = $"Pozycja Myszy - {(int)mx}x, {(int)my}y";
    if (_selectedNode is not null) {
      Title += $" : Wierzchołek - {_selectedNode.Index + 1} - {_selectedNode.X}x, {_selectedNode.Y}y";
    }

    if (_histories.Count > 0 && _selectedNode is not null) {
      var contained = _histories.FirstOrDefault(x => x[HistoryStep].Contains(_selectedNode));
      if (contained is not null) {
        var index = contained[HistoryStep].IndexOf(_selectedNode);
        Title += $" : Indeks - {index}";
      }
    }

    Chart.Refresh();
  }

  private Node? _closestNode;
  private Node? _selectedNode;
  private Instance _instance = null!;
  private string SelectedInstance => Instances.SelectedItem.As<Option>().Value;
  private string SelectedAlgorithm => Algorithms.SelectedItem.As<Option>().Value;
  private int HistoryStep => (int)HistorySlider.Value;
  private readonly ObservableCollection<List<List<Node>>> _histories = new();
  private readonly IPalette _palette = new Category10();
}
