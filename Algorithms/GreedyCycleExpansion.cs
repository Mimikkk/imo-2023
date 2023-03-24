using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyCycleExpansion {
  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
    var population = configuration.Population.ToArray();
    var start = configuration.Start;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      1 => SearchSingle(instance, population.First(), start),
      2 => SearchDouble(instance, population.First(), population.Last(), start),
      _ => instance.SearchMultiple(population)
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(Instance instance, ObservableList<Node> cycle, int? start) {
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Notify();
    cycle.Add(instance.Move.ClosestTo(cycle.First()));
    cycle.Notify();

    while (cycle.Count < instance.Dimension) {
      Moves.AppendFit(cycle, instance.Move.FindBestFitByLowestGain(cycle, cycle));
      cycle.Notify();
    }

    return Yield(cycle);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(Instance instance, ObservableList<Node> first, ObservableList<Node> second, int? start) {
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
      Moves.AppendFit(first, instance.Move.FindBestFitByLowestGain(first, second));
      first.Notify();
      if (first.Count + second.Count == instance.Dimension) break;
      Moves.AppendFit(second, instance.Move.FindBestFitByLowestGain(second, first));
      second.Notify();
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<ObservableList<Node>> cycles) {
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
        Moves.AppendFit(cycle, instance.Move.FindBestFitByLowestGain(cycle, cycles.Flatten().Except(cycle)));
        cycle.Notify();
        if (++count == instance.Dimension) return cycles;
      }
    }
  }
}
