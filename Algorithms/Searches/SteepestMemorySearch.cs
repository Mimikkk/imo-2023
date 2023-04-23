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
      0   => Enumerable.Empty<IEnumerable<Node>>(),
      _   => Multiple(instance, population, gains),
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
        return (neighbourhoods: (va, vb), edge, first, second, gain: 0);
      })
      .ToList();

    var hasMoved = true;
    while (hasMoved) {
      candidates = candidates
        .Where(c => {
          if (c.first == c.second && c.first.Contains(c.edge.a) && c.first.Contains(c.edge.b)) return true;
          if (c.first != c.second && c.first.Contains(c.edge.a) && c.second.Contains(c.edge.b)) return true;
          return false;
        })
        .Select(c => {
          var (_, edge, first, second, _) = c;

          var gain = first == second
            ? instance.Gain.ExchangeEdge(first, edge.a, edge.b)
            : instance.Gain.ExchangeVertex(first, second, edge.a, edge.b);

          return c with { gain = gain };
        })
        .Where(c => c.gain > 0)
        .DistinctBy(c => c.edge)
        .OrderBy(c => c.gain)
        .ToList();

      hasMoved = false;
      Console.WriteLine(candidates.Count);
      Console.WriteLine(cycles.Sum(cycle => instance[cycle]));
      Console.WriteLine();

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
    } else {
      if (!first.Contains(edge.a) || !first.Contains(edge.b)) return Usable.No;
      var cva = first.Neighbourhood(edge.a);
      var cvb = first.Neighbourhood(edge.b);

      if ((pva.a == cva.a && pva.c == cva.c && pvb.a == cvb.a && pvb.c == cvb.c)
          || (pva.a == cva.c && pva.c == cva.a && pvb.a == cvb.c && pvb.c == cvb.a))
        return Usable.Yes;

      if ((pva.a == cva.a && pva.c == cva.c || pva.c == cva.a && pva.a == cva.c)
          && (pvb.a == cvb.c && pvb.c == cvb.a || pvb.a == cvb.c && pvb.c == cvb.a))
        return Usable.No;

      return Usable.No;
    }
  }

  private static void ConsiderMoves(Instance instance, List<List<Node>> cycles,
    List<(((Node a, Node b, Node c) va, (Node a, Node b, Node c) vb) neighbourhoods, (Node a, Node b) edge, List<Node>
      first, List<Node> second, int gain)> candidates,
    (((Node a, Node b, Node c) va, (Node a, Node b, Node c) vb) neighbourhoods, (Node a, Node b) edge, List<Node> first,
      List<Node> second, int gain) candidate) {
    var (_, edge, first, second, gain) = candidate;

    if (first == second) {
      var va = first.Neighbourhood(edge.a);
      var vb = first.Neighbourhood(edge.b);

      foreach (var node in first) {
        var vn = first.Neighbourhood(node);

        if (node != va.c) {
          gain = instance.Gain.ExchangeEdge(first, node, va.c);
          if (gain > 0) {
            var vac = first.Neighbourhood(va.c);
            candidates.Add(((vn, vac), (node, va.c), first, first, gain));
          }
        }

        if (node != va.b) {
          gain = instance.Gain.ExchangeEdge(first, node, va.b);
          if (gain > 0) {
            var vab = first.Neighbourhood(va.b);
            candidates.Add(((vn, vab), (node, va.b), first, first, gain));
          }
        }

        if (node != vb.b) {
          gain = instance.Gain.ExchangeEdge(first, node, vb.b);
          if (gain > 0) {
            var vbb = first.Neighbourhood(vb.b);
            candidates.Add(((vn, vbb), (node, vb.b), first, first, gain));
          }
        }
      }

      foreach (var cycle in cycles.Except(first)) {
        foreach (var node in cycle) {
          var vn = first.Neighbourhood(node);

          gain = instance.Gain.ExchangeVertex(cycle, first, node, va.b);
          if (gain > 0) {
            var vab = first.Neighbourhood(va.b);
            candidates.Add(((vn, vab), (node, va.b), cycle, first, gain));
          }

          gain = instance.Gain.ExchangeVertex(cycle, first, node, va.c);
          if (gain > 0) {
            var vac = first.Neighbourhood(va.c);
            candidates.Add(((vn, vac), (node, va.c), cycle, first, gain));
          }

          gain = instance.Gain.ExchangeVertex(cycle, first, node, vb.b);
          if (gain > 0) {
            var vbb = first.Neighbourhood(vb.b);
            candidates.Add(((vn, vbb), (node, vb.b), cycle, first, gain));
          }
        }
      }
    } else {
      (first, second) = (second, first);
      var va = first.Neighbourhood(edge.a);
      var vb = second.Neighbourhood(edge.b);

      foreach (var node in first) {
        var vn = first.Neighbourhood(node);

        if (node != va.a) {
          gain = instance.Gain.ExchangeEdge(first, node, va.a);
          if (gain > 0) {
            var vaa = first.Neighbourhood(va.a);
            candidates.Add(((vn, vaa), (node, va.a), first, first, gain));
          }
        }

        if (node != va.b) {
          gain = instance.Gain.ExchangeEdge(first, node, va.b);
          if (gain > 0) {
            var vab = first.Neighbourhood(va.b);
            candidates.Add(((vn, vab), (node, va.b), first, first, gain));
          }
        }

        gain = instance.Gain.ExchangeVertex(first, second, node, vb.a);
        if (gain > 0) {
          var vba = second.Neighbourhood(vb.a);
          candidates.Add(((vn, vba), (node, vb.a), first, second, gain));
        }

        gain = instance.Gain.ExchangeVertex(first, second, node, vb.b);
        if (gain > 0) {
          var vbb = second.Neighbourhood(vb.b);
          candidates.Add(((vn, vbb), (node, vb.b), first, second, gain));
        }
      }

      foreach (var node in second) {
        var vn = second.Neighbourhood(node);

        if (node != vb.a) {
          gain = instance.Gain.ExchangeEdge(second, node, vb.a);
          if (gain > 0) {
            var vba = second.Neighbourhood(vb.a);
            candidates.Add(((vn, vba), (node, vb.a), second, second, gain));
          }
        }

        if (node != vb.b) {
          gain = instance.Gain.ExchangeEdge(second, node, vb.b);
          if (gain > 0) {
            var vbb = second.Neighbourhood(vb.b);
            candidates.Add(((vn, vbb), (node, vb.b), second, second, gain));
          }
        }

        gain = instance.Gain.ExchangeVertex(first, second, node, vb.a);
        if (gain > 0) {
          var vba = second.Neighbourhood(vb.a);
          candidates.Add(((vn, vba), (node, vb.a), first, second, gain));
        }

        gain = instance.Gain.ExchangeVertex(first, second, node, vb.b);
        if (gain > 0) {
          var vbb = second.Neighbourhood(vb.b);
          candidates.Add(((vn, vbb), (node, vb.b), first, second, gain));
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
