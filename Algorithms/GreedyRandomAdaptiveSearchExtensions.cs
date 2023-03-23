using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyRandomAdaptiveSearchExtensions {
  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
    var population = configuration.Population.ToArray();
    var start = configuration.Start;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      > 1 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0   => Enumerable.Empty<IEnumerable<Node>>(),
      1   => SearchSingle(instance, population.First(), start),
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(Instance instance, IList<Node>? cycle, int? start) {
    cycle ??= new List<Node>();

    GreedyNearestNeighbourExtensions.Search(instance, new() {
      Population = Yield(cycle),
      Start = start,
    });

    var gains = new Gains(instance);

    // moves.TradeInternalVertices(cycle, cycle[49], cycle[48]);
    // moves.TradeInternalVertices(cycle, cycle[48], cycle[49]);

    // moves.TradeInternalEdges(cycle, cycle[40], cycle[44 + 1]);

    return Yield(cycle);
  }
}
