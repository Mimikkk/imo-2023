using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Structures;
using ScottPlot;
using ScottPlot.Palettes;

namespace Interface.Modules;

internal sealed record MemoryModule(MainWindow Self) {
  public double? Average { get; private set; }
  public void CalculateAverage(int start, int end) {
    Average = Enumerable.Range(start, end)
      .Average(start =>
        I.Algorithm.Search(I.Instance, I.Parameter.Configuration with { Start = start })
          .Sum(nodes => I.Instance.DistanceOf(nodes))
      );
  }

  public void ClearAverage() => Average = null;
  public readonly IPalette Palette = new Category10();
  public readonly ObservableCollection<List<List<Node>>> Histories = new();
  private InteractionModule I => Self.Mod.Interaction;
}
