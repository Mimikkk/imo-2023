using System.Collections;
using Domain;
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
      0   => Enumerable.Empty<IEnumerable<Node>>(),
      _   => Multiple(instance, population, gains),
    };
  }

  private static List<List<Node>> CreateCandidateMatrix(Instance instance, int size) {
    var matrix = new List<List<Node>>(instance.Dimension);

    for (var i = 0; i < matrix.Count; ++i) {
      var vec = new List<(int distance, Node node)>(instance.Dimension);
      for (var j = 0; j < matrix.Count; ++j) vec[j] = (instance[i, j], instance.Nodes[j]);
      vec[i] = vec[i] with { distance = int.MaxValue };

      vec.Sort((a, b) => b.distance - a.distance);

      matrix.Add(vec.Take(size).Select(pair => pair.node).ToList());
    }

    return matrix;
  }

  private static IEnumerable<IEnumerable<Node>>
    Multiple(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var candidate_matrix = CreateCandidateMatrix(instance, 10);

    var enumerable = population.ToArray();

    var cycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var candidates = Moves.Candidates(cycles)
        .Concat(cycles.SelectMany(Moves.Candidates))
        .Select(edge => {
          var first = cycles.Find(c => c.Contains(edge.a))!;
          var second = cycles.Find(c => c.Contains(edge.b))!;

          var gain = first == second
            ? instance.Gain.ExchangeEdge(first, edge.a, edge.b)
            : instance.Gain.ExchangeVertex(first, second, edge.a, edge.b);

          return (edge, first, second, gain);
        });
      var ((a, b), first, second, gain) = candidates.MaxBy(m => m.gain);

      if (gain <= 0) break;
      gains.Add(gain);

      if (first == second) Moves.ExchangeEdge(first, a, b);
      else Moves.ExchangeVertex(first, second, a, b);

      enumerable.Zip(cycles)
        .ForEach(pair => {
          pair.First.Fill(pair.Second);
          pair.First.Notify();
        });
    }

    return cycles;
  }

}
