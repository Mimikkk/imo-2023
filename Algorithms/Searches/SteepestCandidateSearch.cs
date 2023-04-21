using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

internal sealed class SteepestCandidateSearch : ISearch {
  public static IEnumerable<IEnumerable<Node>>
    Search(Instance instance, ISearch.Configuration configuration) {
    var population = configuration.Population.ToArray();
    var initializer = configuration.Initializer;
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

  private static IEnumerable<(Node a, Node b)> CreateCandidates(Instance instance, int size) =>
    instance.Nodes.SelectMany(a =>
        instance.Nodes.Except(a)
          .Select(b => ((a, b), distance: instance[a, b]))
          .OrderBy(edge => edge.distance)
          .Take(size))
      .Select(n => n.DropLast());


  private static IEnumerable<IEnumerable<Node>>
    Multiple(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();
    var cycles = enumerable.Select(solution => solution.ToList()).ToList();
    var candidates = CreateCandidates(instance, 10).ToList();

    while (true) {
      ((List<Node> a, List<Node> b), (Node a, Node b), int gain)? best = null;

      foreach (var (a, b) in candidates) {
        var first = cycles.Find(c => c.Contains(a))!;
        var second = cycles.Find(c => c.Contains(b))!;

        if (first == second) {
          var gain = instance.Gain.ExchangeEdge(first, a, b);
          if (gain > 0 && (!best.HasValue || gain > best.Value.gain)) best = ((first, second), (a, b), gain);
        }
        else {
          var gain = instance.Gain.ExchangeVertex(first, second, a, b);
          if (gain > 0 && (!best.HasValue || gain > best.Value.gain)) best = ((first, second), (a, b), gain);
        }
      }

      if (best.HasValue) {
        var ((first, second), (a, b), gain) = best.Value;
        if (first == second) Moves.ExchangeEdge(first, a, b);
        else Moves.ExchangeVertex(first, second, a, b);

        gains.Add(gain);
        Notify(enumerable, cycles);
      }
      else break;
    }


    return cycles;
  }

  private static void Notify(IEnumerable<ObservableList<Node>> observables, IEnumerable<List<Node>> cycles) {
    observables.Zip(cycles)
      .ForEach(p => {
        p.First.Fill(p.Second);
        p.First.Notify();
      });
  }
}
