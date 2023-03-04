using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using imo_2023.DataStructures;
using imo_2023.Extensions;
using imo_2023.Methods;
using Interface.Types;
using ScottPlot;

namespace Interface;

public partial class MainWindow : Window {
  private string SelectedInstance => Instances.SelectedItem.As<Option>().Value;
  private string SelectedAlgorithm => Algorithms.SelectedItem.As<Option>().Value;
  private int CurrentHistoryStep => (int)HistorySlider.Value;
  private ObservableCollection<List<Node>> History { get; set; } = new();
  private Instance? CurrentInstance = null;
  public MainWindow() {
    InitializeComponent();
    RunStartup();

    History.CollectionChanged += (_, _) => {
      HistorySlider.Maximum = History.Count - 1;
      HistorySlider.Value = History.Count - 1;
    };
    History.Add(new() { new(0, 1, 2), new(1, 1, 2), new(1, 1, 3) });
    History.Add(new() { new(0, 1, 2), new(1, 1, 2), new(1, 5, 8) });
  }

  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;
    HistorySlider.Maximum = 0;
    HistorySlider.PropertyChanged += (_, _) => {
      HistoryText.Text = $"Krok: {CurrentHistoryStep}";
      if (CurrentInstance is null) return;
      Chart.Plot.Clear();
      Chart.Plot.Add.Cycle(History[CurrentHistoryStep], CurrentInstance);
      Chart.Refresh();
    };
    StepNextButton.Click += (_, _) => {
      if (CurrentHistoryStep < History.Count - 1) HistorySlider.Value = CurrentHistoryStep + 1;
    };
    StepBackButton.Click += (_, _) => {
      if (CurrentHistoryStep > 0) HistorySlider.Value = CurrentHistoryStep - 1;
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

  private void InitializeChart() {
    var dataX = new double[] { 1, 2, 3, 4, 5, 6, 7, 8 };
    var dataY = new double[] { 1, 4, 9, 16, 25, 35, 40 };


    Chart.Plot.Add.Scatter(dataX, dataY);
    Chart.Refresh();
  }

  private void RunStartup() {
    GetType()
      .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
      .Where(x => x.Name.StartsWith("Initialize"))
      .ToList()
      .ForEach(x => x.Invoke(this, null));
  }
}
