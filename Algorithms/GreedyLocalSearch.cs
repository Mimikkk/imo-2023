using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyLocalSearch {
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

    var bestCycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var previousBestCycles = bestCycles;
      foreach (var (a, b) in Moves.Candidates(bestCycles).Shuffle()) {
        var cycles = bestCycles.Select(solution => solution.ToList()).ToList();
        var first = cycles.Find(cycle => cycle.Contains(a))!;
        var second = cycles.Find(cycle => cycle.Contains(b))!;

        // Make it so it can take variants
        var gain = instance.Gain.ExchangeVertex(first, second, a, b);

        if (gain <= 0) continue;
        Moves.ExchangeVertex(first, second, a, b);
        bestCycles = cycles;
        enumerable.Zip(cycles).ForEach(p => {
          p.First.Fill(p.Second);
          p.First.Notify();
        });
        break;
      }

      if (previousBestCycles == bestCycles) break;
    }

    return bestCycles;
  }
}
