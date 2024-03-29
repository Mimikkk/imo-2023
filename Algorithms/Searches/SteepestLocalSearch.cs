using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

internal sealed class SteepestLocalSearch : ISearch {
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

    var cycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var (a, b) = Moves.Candidates(cycles)
        .MaxBy(p => {
          var first = cycles.Find(cycle => cycle.Contains(p.a))!;
          var second = cycles.Find(cycle => cycle.Contains(p.b))!;
          return instance.Gain.ExchangeVertex(first, second, p.a, p.b);
        });
      var first = cycles.Find(cycle => cycle.Contains(a))!;
      var second = cycles.Find(cycle => cycle.Contains(b))!;

      var gain = instance.Gain.ExchangeVertex(first, second, a, b);
      if (gain <= 0) break;
      gains.Add(gain);

      Moves.ExchangeVertex(first, second, a, b);
      enumerable.Zip(cycles)
        .ForEach(p => {
          p.First.Fill(p.Second);
          p.First.Notify();
        });
    }

    return cycles;
  }

  private static IEnumerable<IEnumerable<Node>>
    InternalVertices(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var cycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var candidates = cycles.SelectMany(Moves.Candidates)
        .Select(edge => {
          var first = cycles.Find(c => c.Contains(edge.a))!;

          return (edge, first, gain: instance.Gain.ExchangeVertex(first, edge.a, edge.b));
        });
      var ((a, b), cycle, gain) = candidates.MaxBy(m => m.gain);

      if (gain <= 0) break;
      gains.Add(gain);

      Moves.ExchangeVertex(cycle, a, b);
      enumerable.Zip(cycles)
        .ForEach(p => {
          p.First.Fill(p.Second);
          p.First.Notify();
        });
    }

    return cycles;
  }

  private static IEnumerable<IEnumerable<Node>>
    InternalEdges(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var cycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var candidates = cycles.SelectMany(Moves.Candidates)
        .Select(edge => {
          var first = cycles.Find(c => c.Contains(edge.a))!;

          return (edge, first, gain: instance.Gain.ExchangeEdge(first, edge.a, edge.b));
        });
      var ((a, b), cycle, gain) = candidates.MaxBy(m => m.gain);

      if (gain <= 0) break;
      gains.Add(gain);

      Moves.ExchangeEdge(cycle, a, b);
      enumerable.Zip(cycles)
        .ForEach(pair => {
          pair.First.Fill(pair.Second);
          pair.First.Notify();
        });
    }

    return cycles;
  }

  private static IEnumerable<IEnumerable<Node>>
    Vertices(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var cycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var candidates = Moves.Candidates(cycles)
        .Concat(cycles.SelectMany(Moves.Candidates))
        .Select(edge => {
          var first = cycles.Find(c => c.Contains(edge.a))!;
          var second = cycles.Find(c => c.Contains(edge.b))!;
          var gain = first == second
            ? instance.Gain.ExchangeVertex(first, second, edge.a, edge.b)
            : instance.Gain.ExchangeVertex(first, edge.a, edge.b);

          return (edge, first, second, gain);
        });
      var ((a, b), first, second, gain) = candidates.MaxBy(m => m.gain);

      if (gain <= 0) break;
      gains.Add(gain);

      if (first == second) Moves.ExchangeVertex(first, a, b);
      else Moves.ExchangeVertex(first, second, a, b);

      enumerable.Zip(cycles)
        .ForEach(pair => {
          pair.First.Fill(pair.Second);
          pair.First.Notify();
        });
    }

    return cycles;
  }

  private static IEnumerable<IEnumerable<Node>>
    InternalEdgesExternalVertices(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
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

  private static IEnumerable<IEnumerable<Node>>
    Mixed(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();

    var cycles = enumerable.Select(solution => solution.ToList()).ToList();

    while (true) {
      var candidates = Moves.Candidates(cycles)
        .Concat(cycles.SelectMany(Moves.Candidates))
        .Select(edge => {
          var first = cycles.Find(c => c.Contains(edge.a))!;
          var second = cycles.Find(c => c.Contains(edge.b))!;

          var operation = (first == second) switch {
            true when Globals.Random.NextDouble() > 0.5d => "internal-edges",
            true                                         => "internal-vertices",
            _                                            => "external-vertices"
          };

          var gain = operation switch {
            "external-vertices" => instance.Gain.ExchangeVertex(first, second, edge.a, edge.b),
            "internal-vertices" => instance.Gain.ExchangeVertex(first, edge.a, edge.b),
            _                   => instance.Gain.ExchangeEdge(first, edge.a, edge.b)
          };

          return (edge, first, second, operation, gain);
        });
      var ((a, b), first, second, operation, gain) = candidates.MaxBy(m => m.gain);

      if (gain <= 0) break;
      gains.Add(gain);

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

      enumerable.Zip(cycles)
        .ForEach(pair => {
          pair.First.Fill(pair.Second);
          pair.First.Notify();
        });
    }

    return cycles;
  }
}
