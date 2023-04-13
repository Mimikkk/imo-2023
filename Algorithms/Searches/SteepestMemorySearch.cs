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
      .Select(candidate => {
        var first = cycles.Find(c => c.Contains(candidate.a))!;
        var second = cycles.Find(c => c.Contains(candidate.b))!;

        var gain = first == second
          ? instance.Gain.ExchangeEdge(first, candidate.a, candidate.b)
          : instance.Gain.ExchangeVertex(first, second, candidate.a, candidate.b);

        return (edge: candidate, first, second, gain);
      })
      .Where(candidate => candidate.gain > 0)
      .OrderByDescending(candidate => candidate.gain)
      .ToList();

    while (candidates.Count > 0) {
      var ((a, b), first, second, gain) = candidates.MaxBy(m => m.gain).Also(candidates.Remove);

      gains.Add(gain);
      if (first == second) {
        Moves.ExchangeEdge(first, a, b);
        // find moves to remove
        // find new moves to evaluate
        throw new NotImplementedException();
      } else {
        Moves.ExchangeVertex(first, second, a, b);
        // find moves to remove

        var affected = first.Neighbourhood(b)
          .Flat()
          .Concat(first.Neighbourhood(a).Flat())
          .Distinct()
          .ToList();

        foreach (var node in affected) {
           
        }

        // find new moves to evaluate
        throw new NotImplementedException();
      }

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
