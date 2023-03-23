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
    var timeLimit = configuration.TimeLimit;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));
    if (population.Flatten().Count() != instance.Dimension) {
      RandomSearchExtensions.Search(instance, configuration);
    }

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      _ => SearchMultiple(instance, population, timeLimit),
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(Instance instance, IEnumerable<IList<Node>> population, float timeLimit) {
    var bestSolutions = population.Select(solution => solution.ToList()).ToList();
    var bestDistance = bestSolutions.Sum(solution => instance[solution]);

    var timer = Stopwatch.StartNew();
    while (timer.ElapsedMilliseconds < 1000 * timeLimit) {
      var solutions = bestSolutions.Select(solution => solution.ToList()).ToList();
      instance.Move.PerformRandomMove(solutions);

      var distance = solutions.Sum(solution => instance[solution]);
      if (distance >= bestDistance) continue;
      bestDistance = distance;
      bestSolutions = solutions;
    }

    return bestSolutions;
  }
}
