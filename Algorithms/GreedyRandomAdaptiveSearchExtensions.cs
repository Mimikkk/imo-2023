using System.Diagnostics;
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
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      1 => SearchSingle(instance, population.First(), start),
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(Instance instance, IList<Node>? cycle, int? start) {
    cycle ??= new List<Node>();
    var moves = new Moves(instance);

    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));
    return Yield(cycle);
  }

  private record Moves(Instance instance) {
    public static void TradeExternalVertices(IEnumerable<Node> graph) {
    }

    public static void TradeInternalVertices(IEnumerable<Node> graph) {
    }

    public static void TradeInternalEdges(IEnumerable<Node> graph) {
    }
  }
}
