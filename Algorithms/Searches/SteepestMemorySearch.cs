using System.Collections;
using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

internal sealed class SteepestMemorySearch : ISearch {
  public static IEnumerable<IEnumerable<Node>>
    Search(Instance instance, ISearch.Configuration configuration) {
    var population = configuration.Population.ToArray();
    var initializer = configuration.Initializer;
    var variant = configuration.Variant;
    var gains = configuration.Gains;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));
    if (population.Flatten().Count() != instance.Dimension) initializer!.Search(instance, configuration);

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      _ => Multiple(instance, population, gains),
    };
  }


  private static IEnumerable<IEnumerable<Node>>
    Multiple(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();
    var cycles = enumerable.Select(solution => solution.ToList()).ToList();
    var candidates = Moves.Candidates(cycles)
      .Concat(cycles.SelectMany(Moves.Candidates))
      .Select(edge => {
        var first = cycles.Find(c => c.Contains(edge.a))!;
        var second = cycles.Find(c => c.Contains(edge.b))!;

        var va = first.Neighbourhood(edge.a);
        var vb = second.Neighbourhood(edge.b);

        var gain = first == second
          ? instance.Gain.ExchangeEdge(first, edge.a, edge.b)
          : instance.Gain.ExchangeVertex(first, second, edge.a, edge.b);

        return (neighbourhoods: (va, vb), edge, first, second, gain);
      })
      .Where(c => c.gain > 0)
      .ToList();

    var hasMoved = true;
    while (hasMoved) {
      candidates = candidates
        .OrderBy(c => c.gain)
        .ToList();

      hasMoved = false;

      for (var i = candidates.Count - 1; i >= 0; --i) {
        var candidate = candidates[i];
        var usable = IsUsable(candidate);

        if (usable == Usable.Yes) {
          hasMoved = true;

          var (_, edge, first, second, gain) = candidate;
          gains.Add(gain);

          if (first == second) Moves.ExchangeEdge(first, edge.a, edge.b);
          else Moves.ExchangeVertex(first, second, edge.a, edge.b);

          Notify(enumerable, cycles);
          ConsiderMoves(instance, cycles, candidates, candidate);

          candidates.RemoveAt(i);
          break;
        }

        if (usable == Usable.No) candidates.RemoveAt(i);
      }
    }

    return cycles;
  }

  private static Usable IsUsable(
    (((Node a, Node b, Node c) va, (Node a, Node b, Node c) vb) neighbourhoods, (Node a, Node b) edge, List<Node> first,
      List<Node> second, int gain) candidate) {
    var ((pva, pvb), edge, first, second, _) = candidate;

    if (first != second) {
      if (
        (first.Contains(edge.a) && first.Contains(edge.b))
        || (second.Contains(edge.a) && second.Contains(edge.b))
      ) return Usable.No;

      var cva = first.Neighbourhood(edge.a);
      var cvb = second.Neighbourhood(edge.b);

      if ((pva.a == cva.a && pva.c == cva.c || pva.c == cva.a && pva.a == cva.c)
          && (pvb.a == cvb.c && pvb.c == cvb.a || pvb.a == cvb.c && pvb.c == cvb.a))
        return Usable.Yes;

      return Usable.No;
    }
    else {
      if (!first.Contains(edge.a) || !first.Contains(edge.b)) return Usable.No;
      var cva = first.Neighbourhood(edge.a);
      var cvb = first.Neighbourhood(edge.b);

      if ((pva.a == cva.a && pva.c == cva.c && pvb.a == cvb.a && pvb.c == cvb.c)
          || (pva.a == cva.c && pva.c == cva.a && pvb.a == cvb.c && pvb.c == cvb.a))
        return Usable.Yes;

      if ((pva.a == cva.a && pva.c == cva.c || pva.c == cva.a && pva.a == cva.c)
          && (pvb.a == cvb.c && pvb.c == cvb.a || pvb.a == cvb.c && pvb.c == cvb.a))
        return Usable.Maybe;

      return Usable.No;
    }
  }

  private static void ConsiderMoves(Instance instance, List<List<Node>> cycles,
    List<(((Node a, Node b, Node c) va, (Node a, Node b, Node c) vb) neighbourhoods, (Node a, Node b) edge, List<Node>
      first, List<Node> second, int gain)> candidates,
    (((Node a, Node b, Node c) va, (Node a, Node b, Node c) vb) neighbourhoods, (Node a, Node b) edge, List<Node> first,
      List<Node> second, int gain) candidate) {
    var (_, edge, first, second, _) = candidate;

    if (first != second) {
      var va = second.Neighbourhood(edge.a);
      var vb = first.Neighbourhood(edge.b);

      foreach (var (node, vn) in second.Select(node => (node, first.Neighbourhood(node)))) {
        if (node != va.a && instance.Gain.ExchangeEdge(second, node, va.a) is var g1 and > 0) {
          candidates.Add(((vn, first.Neighbourhood(va.a)), (node, va.a), second, first, g1));
        }

        if (node != va.b && instance.Gain.ExchangeEdge(second, node, va.a) is var g2 and > 0) {
          candidates.Add(((vn, first.Neighbourhood(va.b)), (node, va.b), second, first, g2));
        }

        if (instance.Gain.ExchangeVertex(second, first, node, vb.a) is var g3 and > 0) {
          candidates.Add(((vn, first.Neighbourhood(vb.a)), (node, vb.a), second, first, g3));
        }

        if (instance.Gain.ExchangeVertex(second, first, node, vb.b) is var g4 and > 0) {
          candidates.Add(((vn, first.Neighbourhood(vb.b)), (node, vb.b), second, first, g4));
        }
      }

      foreach (var (node, vn) in first.Select(node => (node, first.Neighbourhood(node)))) {
        if (node != vb.a && instance.Gain.ExchangeEdge(first, node, vb.a) is var g1 and > 0) {
          candidates.Add(((vn, first.Neighbourhood(vb.a)), (node, vb.a), first, first, g1));
        }

        if (node != vb.b && instance.Gain.ExchangeEdge(first, node, vb.b) is var g2 and > 0) {
          candidates.Add(((vn, first.Neighbourhood(vb.b)), (node, vb.b), first, first, g2));
        }

        if (instance.Gain.ExchangeVertex(first, second, node, va.a) is var g3 and > 0) {
          candidates.Add(((vn, second.Neighbourhood(va.a)), (node, va.a), first, second, g3));
        }

        if (instance.Gain.ExchangeVertex(first, second, node, va.b) is var g4 and > 0) {
          candidates.Add(((vn, second.Neighbourhood(va.b)), (node, va.b), first, second, g4));
        }
      }
    }
    else {
      var va = first.Neighbourhood(edge.a);
      var vb = first.Neighbourhood(edge.b);

      foreach (var (node, vn) in first.Select(node => (node, first.Neighbourhood(node)))) {
        if (node != va.b && instance.Gain.ExchangeEdge(second, node, va.b) is var g1 and > 0) {
          candidates.Add(((vn, second.Neighbourhood(va.c)), (node, va.c), second, second, g1));
        }

        if (node != va.b && instance.Gain.ExchangeEdge(second, node, va.b) is var g2 and > 0) {
          candidates.Add(((vn, second.Neighbourhood(va.b)), (node, va.b), second, second, g2));
        }

        if (node != vb.b && instance.Gain.ExchangeEdge(second, node, vb.b) is var g3 and > 0) {
          candidates.Add(((vn, first.Neighbourhood(vb.b)), (node, vb.b), second, second, g3));
        }
      }

      second = cycles.Find(cycle => cycle != first);

      if (second is null) return;
      foreach (var (node, vn) in second.Select(node => (node, second.Neighbourhood(node)))) {
        if (instance.Gain.ExchangeVertex(second, first, node, va.b) is var g1 and > 0) {
          candidates.Add(((vn, second.Neighbourhood(va.b)), (node, va.b), second, first, g1));
        }

        if (instance.Gain.ExchangeVertex(second, first, node, va.c) is var g2 and > 0) {
          candidates.Add(((vn, second.Neighbourhood(va.c)), (node, va.c), second, first, g2));
        }

        if (instance.Gain.ExchangeVertex(second, first, node, vb.b) is var g3 and > 0) {
          candidates.Add(((vn, second.Neighbourhood(vb.b)), (node, vb.b), second, first, g3));
        }
      }
    }
  }


  private static void Notify(IEnumerable<ObservableList<Node>> observables, IEnumerable<List<Node>> cycles) {
    observables.Zip(cycles)
      .ForEach(p => {
        p.First.Fill(p.Second);
        p.First.Notify();
      });
  }


  private enum Usable {
    Yes,
    No,
    Maybe
  }
}
