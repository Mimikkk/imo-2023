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

  private record Gains(Instance Instance) {
    public int ReplaceInternalVertex(IEnumerable<Node> cycle, Node replace, Node with) {
      var vertex = cycle.Neighbourhood(with);

      return Instance[vertex] - Instance[(vertex.a, replace, vertex.c)];
    }

    public int ReplaceExternalTwoVertices(IEnumerable<Node> first, IEnumerable<Node> second, Node a, Node b) =>
      ReplaceInternalVertex(first, a, b) + ReplaceInternalVertex(second, b, a);

    public int InsertCost((Node a, Node b) edge, Node node) =>
      Instance[edge.a, node] + Instance[node, edge.b] - Instance[edge];

    public int ReplaceInternalTwoVertices(IEnumerable<Node> cycle, Node a, Node b) {
      cycle = cycle.ToArray();

      return ReplaceExternalTwoVertices(cycle, cycle, a, b);
    }
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
