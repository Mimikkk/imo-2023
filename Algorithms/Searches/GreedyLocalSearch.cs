using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

internal sealed class GreedyLocalSearch : ISearch {
  public static IEnumerable<IEnumerable<Node>>
    Search(Instance instance, ISearch.Configuration configuration) {
    var population = configuration.Population.ToArray();
    var initializer = configuration.Initializer;
    var variant = configuration.Variant;
    var gains = configuration.Gains;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));
    if (population.Flatten().Count() != instance.Dimension) initializer!.Search(instance, configuration);

    return (population.Length, variant) switch {
      (< 0, _)                                => throw new ArgumentOutOfRangeException(nameof(configuration)),
      (0, _)                                  => Enumerable.Empty<IEnumerable<Node>>(),
      (_, "internal-vertices")                => InternalVertices(instance, population, gains),
      (_, "external-vertices")                => ExternalVertices(instance, population, gains),
      (_, "internal-edges")                   => InternalEdges(instance, population, gains),
      (_, "vertices")                         => Vertices(instance, population, gains),
      (_, "external-vertices-internal-edges") => InternalEdgesExternalVertices(instance, population, gains),
      (_, "mixed")                            => Mixed(instance, population, gains),
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    ExternalVertices(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var bestCycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var previousBestCycles = bestCycles;
      var candidates = Moves.Candidates(bestCycles).Shuffle();

      foreach (var (a, b) in candidates) {
        var cycles = bestCycles.Select(solution => solution.ToList()).ToList();
        var first = cycles.Find(cycle => cycle.Contains(a))!;
        var second = cycles.Find(cycle => cycle.Contains(b))!;

        var gain = instance.Gain.ExchangeVertex(first, second, a, b);

        if (gain <= 0) continue;
        Moves.ExchangeVertex(first, second, a, b);
        gains.Add(gain);

        bestCycles = cycles;
        enumerable.Zip(cycles)
          .ForEach(p => {
            p.First.Fill(p.Second);
            p.First.Notify();
          });
        break;
      }

      if (previousBestCycles == bestCycles) break;
    }

    return bestCycles;
  }

  private static IEnumerable<IEnumerable<Node>>
    InternalVertices(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var bestCycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var previousBestCycles = bestCycles;
      foreach (var (a, b) in bestCycles.SelectMany(Moves.Candidates).Shuffle()) {
        var cycles = bestCycles.Select(solution => solution.ToList()).ToList();
        var cycle = cycles.Find(cycle => cycle.Contains(a))!;

        var gain = instance.Gain.ExchangeVertex(cycle, a, b);

        if (gain <= 0) continue;
        Moves.ExchangeVertex(cycle, a, b);
        gains.Add(gain);

        bestCycles = cycles;
        enumerable.Zip(cycles)
          .ForEach(p => {
            p.First.Fill(p.Second);
            p.First.Notify();
          });
        break;
      }

      if (previousBestCycles == bestCycles) break;
    }

    return bestCycles;
  }

  private static IEnumerable<IEnumerable<Node>>
    InternalEdges(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var bestCycles = enumerable.Select(solution => solution.ToList()).ToList();


    while (true) {
      var previousBestCycles = bestCycles;
      foreach (var (a, b) in bestCycles.SelectMany(Moves.Candidates).Shuffle()) {
        var cycles = bestCycles.Select(solution => solution.ToList()).ToList();
        var cycle = cycles.Find(cycle => cycle.Contains(a))!;

        var gain = instance.Gain.ExchangeEdge(cycle, a, b);

        if (gain <= 0) continue;
        Moves.ExchangeEdge(cycle, a, b);
        gains.Add(gain);

        bestCycles = cycles;
        enumerable.Zip(cycles)
          .ForEach(p => {
            p.First.Fill(p.Second);
            p.First.Notify();
          });
        break;
      }

      if (previousBestCycles == bestCycles) break;
    }

    return bestCycles;
  }


  private static IEnumerable<IEnumerable<Node>>
    Vertices(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var bestCycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var previousBestCycles = bestCycles;

      foreach (var (a, b) in Moves.Candidates(bestCycles).Concat(bestCycles.SelectMany(Moves.Candidates)).Shuffle()) {
        var cycles = bestCycles.Select(solution => solution.ToList()).ToList();
        var first = cycles.Find(cycle => cycle.Contains(a))!;
        var second = cycles.Find(cycle => cycle.Contains(b))!;


        var gain = first == second
          ? instance.Gain.ExchangeVertex(first, a, b)
          : instance.Gain.ExchangeVertex(first, second, a, b);

        if (gain <= 0) continue;
        gains.Add(gain);

        if (first == second) Moves.ExchangeVertex(first, a, b);
        else Moves.ExchangeVertex(first, second, a, b);

        bestCycles = cycles;
        enumerable.Zip(cycles)
          .ForEach(p => {
            p.First.Fill(p.Second);
            p.First.Notify();
          });
        break;
      }

      if (previousBestCycles == bestCycles) break;
    }

    return bestCycles;
  }

  private static IEnumerable<IEnumerable<Node>>
    InternalEdgesExternalVertices(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var bestCycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var previousBestCycles = bestCycles;

      foreach (var (a, b) in Moves.Candidates(bestCycles).Concat(bestCycles.SelectMany(Moves.Candidates)).Shuffle()) {
        var cycles = bestCycles.Select(solution => solution.ToList()).ToList();
        var first = cycles.Find(cycle => cycle.Contains(a))!;
        var second = cycles.Find(cycle => cycle.Contains(b))!;

        var gain = first == second
          ? instance.Gain.ExchangeEdge(first, a, b)
          : instance.Gain.ExchangeVertex(first, second, a, b);

        if (gain <= 0) continue;
        gains.Add(gain);

        if (first == second) Moves.ExchangeEdge(first, a, b);
        else Moves.ExchangeVertex(first, second, a, b);


        bestCycles = cycles;
        enumerable.Zip(cycles)
          .ForEach(p => {
            p.First.Fill(p.Second);
            p.First.Notify();
          });
        break;
      }

      if (previousBestCycles == bestCycles) break;
    }

    return bestCycles;
  }

  private static IEnumerable<IEnumerable<Node>>
    Mixed(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var bestCycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var previousBestCycles = bestCycles;

      foreach (var (a, b) in Moves.Candidates(bestCycles).Concat(bestCycles.SelectMany(Moves.Candidates)).Shuffle()) {
        var cycles = bestCycles.Select(solution => solution.ToList()).ToList();
        var first = cycles.Find(cycle => cycle.Contains(a))!;
        var second = cycles.Find(cycle => cycle.Contains(b))!;

        var operation = (first == second) switch {
          true when Globals.Random.NextDouble() > 0.5d => "internal-edges",
          true                                         => "internal-vertices",
          _                                            => "external-vertices"
        };

        var gain = operation switch {
          "external-vertices" => instance.Gain.ExchangeVertex(first, second, a, b),
          "internal-vertices" => instance.Gain.ExchangeVertex(first, a, b),
          _                   => instance.Gain.ExchangeEdge(first, a, b)
        };

        if (gain <= 0) continue;
        switch (operation) {
          case "external-vertices":
            Moves.ExchangeVertex(first, second, a, b);
            break;
          case "internal-vertices":
            Moves.ExchangeVertex(first, a, b);
            break;
          default:
            Moves.ExchangeEdge(first, a, b);
            break;
        }

        gains.Add(gain);

        bestCycles = cycles;
        enumerable.Zip(cycles)
          .ForEach(p => {
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
