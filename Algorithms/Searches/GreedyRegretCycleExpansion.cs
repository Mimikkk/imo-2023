using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

internal sealed class GreedyRegretCycleExpansion : ISearch {
  public static IEnumerable<IEnumerable<Node>>
    Search(Instance instance, ISearch.Configuration configuration) {
    var population = configuration.Population.ToArray();
    var regret = configuration.Regret;
    var start = configuration.Start;

    if (configuration.Regret == 0)
      throw new ArgumentNullException(nameof(configuration.Regret),
        "Regret must be specified for greedy regret cycle expansion with k-regrets.");

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration), "Population must be non-negative."),
      0   => Enumerable.Empty<IEnumerable<Node>>(),
      1   => Single(instance, population.First(), start, regret),
      2   => Double(instance, population.First(), population.Last(), start, regret),
      _   => Multiple(instance, population, regret)
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    Single(Instance instance, ObservableList<Node> cycle, int? start, int regret) {
    cycle.Add(start is null ? Globals.Random.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Notify();
    cycle.Add(instance.Move.ClosestTo(cycle.First()));
    cycle.Notify();

    while (cycle.Count < instance.Dimension) {
      Moves.AppendFit(cycle, instance.Move.FindBestFitByRegretInsertGain(cycle, cycle, regret));
      cycle.Notify();
    }

    return Yield(cycle);
  }

  private static IEnumerable<IEnumerable<Node>>
    Double(Instance instance, ObservableList<Node> first, ObservableList<Node> second, int? start,
      int regret) {
    first.Add(start is null ? Globals.Random.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Notify();
    first.Add(instance.Move.ClosestTo(first.First()));
    first.Notify();

    second.Add(instance.Move.FurthestTo(first.First()));
    second.Notify();
    second.Add(instance.Move.ClosestTo(second.First()));
    second.Notify();

    while (true) {
      if (first.Count + second.Count == instance.Dimension) break;
      Moves.AppendFit(first, instance.Move.FindBestFitByRegretInsertGain(first, second, regret));
      first.Notify();
      if (first.Count + second.Count == instance.Dimension) break;
      Moves.AppendFit(second, instance.Move.FindBestFitByRegretInsertGain(second, first, regret));
      second.Notify();
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    Multiple(Instance instance, IEnumerable<ObservableList<Node>> cycles, int regret) {
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
        Moves.AppendFit(cycle, instance.Move.FindBestFitByRegretInsertGain(cycle, cycles.Flatten(), regret));
        cycle.Notify();
        if (++count == instance.Dimension) return cycles;
      }
    }
  }
}
