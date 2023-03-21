using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;
using Interface.Types;
using ScottPlot;
using ScottPlot.Palettes;
using static System.Linq.Enumerable;

namespace Interface;

public sealed partial class MainWindow {
  private record Memorized(MainWindow Self) {
    public double? Average => _average;
    public void CalculateAverage(int start, int end) {
      _average = Range(start, end)
        .Average(start =>
          Self.Interaction.Algorithm.Search(Self.Interaction.Instance, Self.Interaction.Parameter.Configuration with { Start = start })
            .Sum(nodes => Self.Interaction.Instance.DistanceOf(nodes))
        );
    }
    public void ClearAverage() => _average = null;
    private double? _average;
    public readonly IPalette _palette = new Category10();
    public readonly ObservableCollection<List<List<Node>>> Histories = new();

  }

  private readonly Memorized Memory;
}
