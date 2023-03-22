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
    var moves = new Moves(instance);

    // moves.TradeInternalVertices(cycle, cycle[49], cycle[48]);
    // moves.TradeInternalVertices(cycle, cycle[48], cycle[49]);

    moves.TradeInternalEdges(cycle, (cycle[39], cycle[40]), (cycle[48], cycle[49]));

    return Yield(cycle);
  }

  private record Gains(Instance Instance) {
    public int Insert((Node a, Node b) edge, Node node) => Instance[(edge.a, node, edge.b)] - Instance[edge];

    public int InternalVertex(IEnumerable<Node> cycle, Node replace, Node with) {
      var vertex = cycle.Neighbourhood(with);

      return Instance[vertex] - Instance[(vertex.a, replace, vertex.c)];
    }

    public int ExternalTwoVertices(IEnumerable<Node> first, IEnumerable<Node> second, Node a, Node b) =>
      InternalVertex(first, a, b) + InternalVertex(second, b, a);

    public int ReplaceInternalTwoVertices(IEnumerable<Node> cycle, Node a, Node b) {
      cycle = cycle.ToArray();

      return ExternalTwoVertices(cycle, cycle, a, b);
    }
  }

  private record Moves(Instance instance) {
    public void TradeExternalVertices(IList<Node> first, IList<Node> second, Node a, Node b) {
      var ia = first.IndexOf(a);
      var ib = second.IndexOf(b);

      first.Remove(a);
      first.Insert(ia, b);
      second.Remove(b);
      second.Insert(ib, a);
    }

    public void TradeInternalVertices(IList<Node> cycle, Node a, Node b) => cycle.Swap(a, b);

    public void TradeInternalEdges(IList<Node> cycle, (Node a, Node b) a, (Node a, Node b) b) {
      var ia = cycle.IndexOf(a.a);
      var ib = cycle.IndexOf(b.a);
      (ia, ib) = ia > ib ? (ib, ia) : (ia, ib);
      cycle.Swap(ia, ib);

      while (ia < ib) {
        (cycle[ia], cycle[ib]) = (cycle[ib], cycle[ia]);
        ++ia;
        --ib;
      }
    }
  }
}
