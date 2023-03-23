using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyRegretCycleExpansionExtensions {
  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
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
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      1 => instance.SearchSingle(population.First(), start, regret),
      2 => instance.SearchDouble(population.First(), population.Last(), start, regret),
      _ => instance.SearchMultiple(population, regret)
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(this Instance instance, IList<Node>? cycle, int? start, int regret) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.Move.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension) {
      Moves.AppendFit(cycle, instance.Move.FindBestFitByRegretInsertGain(cycle, cycle, regret));
    }

    return Yield(cycle);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(this Instance instance, IList<Node>? first, IList<Node>? second, int? start, int regret) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Add(instance.Move.ClosestTo(first.First()));

    second ??= new List<Node>();
    second.Add(instance.Move.FurthestTo(first.First()));
    second.Add(instance.Move.ClosestTo(second.First()));

    while (first.Count < instance.Dimension / 2) {
      Moves.AppendFit(first, instance.Move.FindBestFitByRegretInsertGain(first, second, regret));
      Moves.AppendFit(second, instance.Move.FindBestFitByRegretInsertGain(second, first, regret));
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<IList<Node>> cycles, int regret) {
    cycles = cycles.ToArray();

    var points = instance.Move.FindFurthest(cycles.Count());
    foreach (var (path, point) in cycles.Zip(points)) path.Add(point);
    foreach (var path in cycles) instance.Move.ClosestTo(path.First(), cycles.Flatten());

    var count = cycles.Flatten().Count();
    while (true) {
      foreach (var cycle in cycles) {
        Moves.AppendFit(cycle, instance.Move.FindBestFitByRegretInsertGain(cycle, cycles.Flatten(), regret));
        if (++count == instance.Dimension) return cycles;
      }
    }
  }
}
