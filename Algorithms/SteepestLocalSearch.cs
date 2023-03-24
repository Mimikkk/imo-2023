using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class SteepestLocalSearch {
  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
    var population = configuration.Population.ToArray();
    var initializer = configuration.Initializer;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));
    if (population.Flatten().Count() != instance.Dimension) initializer!.Search(instance, configuration);

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      _ => SearchMultiple(instance, population),
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(Instance instance, IEnumerable<ObservableList<Node>> population) {
    var enumerable = population.ToArray();

    var cycles = enumerable.Select(solution => solution.ToList()).ToList();
    while (true) {
      var (a, b) = Moves.Candidates(cycles).MaxBy(p => {
        var first = cycles.Find(cycle => cycle.Contains(p.a))!;
        var second = cycles.Find(cycle => cycle.Contains(p.b))!;
        return instance.Gain.ExchangeVertex(first, second, p.a, p.b);
      });
      var first = cycles.Find(cycle => cycle.Contains(a))!;
      var second = cycles.Find(cycle => cycle.Contains(b))!;

      if (instance.Gain.ExchangeVertex(first, second, a, b) <= 0) break;

      // Make it so it can take variants
      Moves.ExchangeVertex(first, second, a, b);
      enumerable.Zip(cycles).ForEach(p => {
        p.First.Fill(p.Second);
        p.First.Notify();
      });
    }

    return cycles;
  }
}
