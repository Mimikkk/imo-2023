using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Algorithms.Structures;
using Avalonia.Controls;
using Avalonia.Input;
using Domain.Extensions;
using Domain.Structures;
using Interface.Modules;
using Interface.Types;
using static System.Linq.Enumerable;

namespace Interface;

public sealed partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();

    Mod = new(this);
    C.Subscribe(T.Update);

    InitializeComboBoxes();
    InitializeChart();
    InitializeListeners();
  }

  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;


    M.Histories.CollectionChanged += (_, _) => C.Notify();

    HistorySlider.PropertyChanged += (_, change) => {
      if (change.Property.Name != "Value") return;

      HistoryText.Text = $"Krok: {I.Step}";
      C.Notify();
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
        .MinBy(start => I.Algorithm.Search(
            I.Instance,
            I.Parameter.Configuration with { Start = start })
          .Sum(nodes => I.Instance[nodes])
        );
      HandleRunCommand();
    };
    FindWorstButton.Click += (_, _) => {
      ParameterStartIndex.Value = Range((int)ParameterStartIndex.Minimum + 1, (int)ParameterStartIndex.Maximum - 1)
        .MinBy(start => I.Algorithm.Search(
            I.Instance,
            I.Parameter.Configuration with { Start = start })
          .Sum(nodes => I.Instance[nodes])
        );
      HandleRunCommand();
    };
    CalculateAverageButton.Click += (_, _) => {
      M.CalculateAverage((int)ParameterStartIndex.Minimum + 1, (int)ParameterStartIndex.Maximum - 1);
      C.Notify();
    };
  }

  private void InitializeComboBoxes() {
    Instances.SelectionChanged += (_, _) => {
      ParameterStartIndex.Maximum = I.Instance.Dimension;
      ParameterPopulationSize.Maximum = I.Hull.Count() - 1;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
      HistorySlider.Value = 0;
      HistorySlider.Maximum = 0;

      Chart.Plot.AutoScale();
      M.ClearAverage();
      M.Histories.Clear();
    };
    Instances.Items = new List<Option<string>> {
      new("KroA 100", "kroA100"),
      new("KroB 100", "kroB100")
    };
    Instances.SelectedIndex = 0;

    ParameterStartIndex.ValueChanged += (_, _) => {
      ParameterPopulationSize.Maximum = I.Parameter.StartIndex > I.Hull.Count()
        ? 2
        : I.Hull.Count() - 1;
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
    };
    ParameterPopulationSize.ValueChanged += (_, _) => {
      ParameterStartIndex.Maximum = I.Parameter.PopulationSize > 2
        ? I.Hull.Count() - 1
        : I.Instance.Dimension;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);

      if (I.Algorithm.UsesVariant) {
        var size = I.Parameter.PopulationSize;

        ParameterVariants.Items = size switch {
          1 => new List<Option<string>> {
            new("Wewnętrzna wymiana wierzchołków", "internal-vertices"),
            new("Wewnętrzna wymiana krawędzi", "internal-edges")
          },
          _ => new List<Option<string>> {
            new("Wewnętrzna wymiana wierzchołków", "internal-vertices"),
            new("Zewnętrzna wymiana wierzchołków", "external-vertices"),
            new("Wewnętrzna wymiana krawędzi", "internal-edges"),
            new("Mieszany", "mixed")
          }
        };
        ParameterVariants.SelectedIndex = 0;
      }
    };

    Algorithms.SelectionChanged += (_, _) => {
      ParameterRegretBox.IsVisible = I.Algorithm.UsesRegret;
      ParameterWeightBox.IsVisible = I.Algorithm.UsesWeight;
      ParameterTimeLimitBox.IsVisible = I.Algorithm.UsesTimeLimit;
      ParameterInitializersBox.IsVisible = I.Algorithm.UsesInitializer;
      ParameterVariantsBox.IsVisible = I.Algorithm.UsesVariant;
      if (I.Algorithm.UsesVariant) {
        var size = I.Parameter.PopulationSize;

        ParameterVariants.Items = size switch {
          1 => new List<Option<string>> {
            new("Wewnętrzna wymiana wierzchołków", "internal-vertices"),
            new("Wewnętrzna wymiana krawędzi", "internal-edges")
          },
          _ => new List<Option<string>> {
            new("Wewnętrzna wymiana wierzchołków", "internal-vertices"),
            new("Zewnętrzna wymiana wierzchołków", "external-vertices"),
            new("Wewnętrzna wymiana krawędzi", "internal-edges"),
            new("Mieszany", "mixed")
          }
        };
        ParameterVariants.SelectedIndex = 0;
      }


      ParameterRegret.Value = 2;
      M.ClearAverage();
      C.Notify();
    };
    Algorithms.Items = new List<Option<Algorithm>> {
      new("Najbliższy sąsiad", Algorithm.NearestNeighbour),
      new("Rozszerzanie cyklu", Algorithm.CycleExpansion),
      new("Rozszerzanie cyklu z k-żalem", Algorithm.CycleExpansionWithKRegret),
      new("Rozszerzanie cyklu z ważonym k-żalem", Algorithm.CycleExpansionWithKRegretAndWeight),
      new("Zachłanne sąsiedztwo", Algorithm.GreedyLocal),
      new("Strome sąsiedztwo", Algorithm.SteepestLocal),
      new("Przypadkowe próbkowanie", Algorithm.Random),
      new("GRASP", Algorithm.RandomAdaptive)
    };
    Algorithms.SelectedIndex = 0;

    ParameterVariants.Items = new List<Option<string>>();


    ParameterInitializers.Items = new List<Option<Algorithm>> {
      new("Przypadkowe próbkowanie", Algorithm.Random),
      new("Rozszerzanie z k-żalem", Algorithm.CycleExpansionWithKRegretAndWeight)
    };
    ParameterInitializers.SelectedIndex = 0;
  }

  private void InitializeChart() {
    Chart.Plot.AutoScale();
    C.Notify();

    Chart.PointerMoved += (_, _) => {
      Mouse.UpdateClosest();
      C.Notify();
      P.Notify();
    };
    Chart.PointerReleased += (_, e) => {
      (e.KeyModifiers == KeyModifiers.Control)
        .And(Mouse.UpdateSelection)
        .Or(Mouse.UpdateSelected);
      C.Notify();
      P.Notify();
    };
  }

  private void HandleRunCommand() {
    M.Histories.Clear();

    var histories = Times(I.Parameter.PopulationSize, () => new List<List<Node>> { new() })
      .ToList();

    var timer = Stopwatch.StartNew();
    I.Algorithm.Search(I.Instance, I.Parameter.Configuration with {
      Population = histories.Select(history => new ObservableList<Node>(items => history.Add(items.ToList()))).ToList()
    });

    timer.Stop();
    var elapsed = timer.ElapsedMilliseconds;
    ParameterTimeLimit.Value = Math.Round((float)elapsed / 1000, 2);

    histories.ForEach(M.Histories.Add);
    HistorySlider.Maximum = histories.MaxBy(x => x.Count)!.Count - 1;
    HistorySlider.Value = HistorySlider.Maximum;
  }

  internal readonly Modules.Modules Mod;
  private MouseModule Mouse => Mod.Mouse;
  private TitleModule T => Mod.Title;
  private MemoryModule M => Mod.Memory;
  private ChartRendererModule C => Mod.Chart;
  private InteractionModule I => Mod.Interaction;
  private CyclePanelModule P => Mod.Panel;
}
