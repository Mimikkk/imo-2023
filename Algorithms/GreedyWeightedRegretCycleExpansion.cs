using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms;

internal static class GreedyWeightedRegretCycleExpansion {
  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
    var population = configuration.Population.ToArray();
    var weight = configuration.Weight;
    var regret = configuration.Regret;
    var start = configuration.Start;

    if (configuration.Regret == 0)
      throw new ArgumentNullException(nameof(configuration.Regret),
        "Regret must be specified for greedy regret cycle expansion with k-regrets.");

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration), "Population must be non-negative."),
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      1 => instance.SearchSingle(population.First(), start, regret, weight),
      2 => instance.SearchDouble(population.First(), population.Last(), start, regret, weight),
      _ => instance.SearchMultiple(population, regret, weight)
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(this Instance instance, ObservableList<Node> cycle, int? start, int regret, float weight) {
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Notify();
    cycle.Add(instance.Move.ClosestTo(cycle.First()));
    cycle.Notify();

    while (cycle.Count < instance.Dimension) {
      Moves.AppendFit(cycle, instance.Move.FindBestFitByWeightedRegretToInsertGain(cycle, cycle, regret, weight));
      cycle.Notify();
    }

    return Yield(cycle);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(this Instance instance, ObservableList<Node> first, ObservableList<Node> second, int? start,
      int regret,
      float weight) {
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Notify();
    first.Add(instance.Move.ClosestTo(first.First()));
    first.Notify();

    second.Add(instance.Move.FurthestTo(first.First()));
    second.Notify();
    second.Add(instance.Move.ClosestTo(second.First()));
    second.Notify();

    while (true) {
      if (first.Count + second.Count == instance.Dimension) break;
      Moves.AppendFit(first, instance.Move.FindBestFitByWeightedRegretToInsertGain(first, second, regret, weight));
      first.Notify();

      if (first.Count + second.Count == instance.Dimension) break;
      Moves.AppendFit(second, instance.Move.FindBestFitByWeightedRegretToInsertGain(second, first, regret, weight));
      second.Notify();
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<ObservableList<Node>> cycles, int regret, float weight) {
    cycles = cycles.ToArray();

    foreach (var (cycle, point) in cycles.Zip(instance.Move.FindFurthest(cycles.Count()))) {
      cycle.Add(point);
      cycle.Notify();
    }

    foreach (var cycle in cycles) {
      instance.Move.ClosestTo(cycle.First(), cycles.Flatten());
      cycle.Notify();
    }

    var count = cycles.Flatten().Count();
    while (true) {
      foreach (var cycle in cycles) {
        Moves.AppendFit(
          cycle,
          instance.Move.FindBestFitByWeightedRegretToInsertGain(cycle, cycles.Flatten(), regret, weight)
        );
        cycle.Notify();
        if (++count == instance.Dimension) return cycles;
      }
    }
  }
}
