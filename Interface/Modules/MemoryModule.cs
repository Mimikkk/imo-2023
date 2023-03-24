using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Domain.Structures;
using ScottPlot;
using ScottPlot.Palettes;

namespace Interface.Modules;

internal sealed record MemoryModule(MainWindow Self) {
  public double? AverageScore { get; private set; }
  public double? BestScore { get; private set; }
  public double? WorstScore { get; private set; }
  public double? AverageTime { get; private set; }
  public double? BestTime { get; private set; }
  public double? WorstTime { get; private set; }

  public void CalculateAverage(int start, int end) {
    var measurements = Enumerable.Range(start, end)
      .Select(index => {
        var timer = Stopwatch.StartNew();

        var distance = I.Algorithm.Search(I.Instance, I.Parameter.Configuration with { Start = index })
          .Sum(nodes => I.Instance[nodes]);

        var time = timer.ElapsedMilliseconds;

        return (distance, time);
      }).ToList();


    AverageScore = measurements.Average(m => m.distance);
    AverageTime = measurements.Average(m => m.time);
    WorstScore = measurements.Max(m => m.distance);
    WorstTime = measurements.Max(m => m.time);
    BestScore = measurements.Min(m => m.distance);
    BestTime = measurements.Min(m => m.time);
  }

  public void ClearAverage() {
    AverageScore = null;
    AverageTime = null;
  }

  public readonly IPalette Palette = new Category10();
  public readonly ObservableCollection<List<List<Node>>> Histories = new();
  private InteractionModule I => Self.Mod.Interaction;
}
