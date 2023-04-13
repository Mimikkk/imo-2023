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

    var candidates =
      // Moves.Candidates(cycles) // External
      // .Concat(cycles.SelectMany(Moves.Candidates)) // Internal
      Moves.Candidates(cycles)
        .Select(nodes => {
          var first = cycles.Find(c => c.Contains(nodes.a))!;
          var second = cycles.Find(c => c.Contains(nodes.b))!;

          var gain = first == second
            ? instance.Gain.ExchangeEdge(first, nodes.a, nodes.b)
            : instance.Gain.ExchangeVertex(first, second, nodes.a, nodes.b);

          return (nodes, first, second, gain);
        })
        .Where(candidate => candidate.gain > 0)
        .OrderByDescending(candidate => candidate.gain)
        .ToList();

    while (true) {
      ((Node a, Node b) nodes, List<Node> first, List<Node> second, int gain)? best = null;

      var removables = new List<((Node a, Node b) nodes, List<Node> first, List<Node> second, int gain)>();
      foreach (var candidate in candidates) {
        var ((a_, b_), first_, second_, _) = candidate;
        if (first_ == second_) {
          // swap edge
          throw new NotImplementedException();
        } else {
          // swap vertices

          var va = first_.Neighbourhood(a_);
          var vb = second_.Neighbourhood(b_);
          var o1 = OrientationOf(first_, va.a, va.b);
          var o2 = OrientationOf(first_, va.b, va.c);
          var o3 = OrientationOf(second_, vb.a, vb.b);
          var o4 = OrientationOf(second_, vb.b, vb.c);

          if (Yield(o1, o2, o3, o4).Contains(null)) {
            removables.Add(candidate);
          } else if (o1 == o2 && o3 == o4) {
            removables.Add(candidate);
            best = candidate;
            break;
          }
        }
      }
      removables.ForEach(candidate => candidates.Remove(candidate));

      if (!best.HasValue) return cycles;
      var ((a, b), first, second, gain) = best.Value;

      gains.Add(gain);
      if (first == second) Moves.ExchangeEdge(first, a, b);
      else Moves.ExchangeVertex(first, second, a, b);

      enumerable.Zip(cycles)
        .ForEach(pair => {
          pair.First.Fill(pair.Second);
          pair.First.Notify();
        });

      candidates.Sort((a, b) => b.gain - a.gain);
    }

    return cycles;
  }
}
