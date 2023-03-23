using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyWeightedRegretCycleExpansionExtensions {
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
    SearchSingle(this Instance instance, IList<Node>? cycle, int? start, int regret, float weight) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.Move.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension) {
      var (previous, best) = FindFitsByRegretGain(instance, cycle, cycle, regret, weight);
      cycle.Insert(cycle.IndexOf(previous), best);
    }

    return Yield(cycle);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(this Instance instance, IList<Node>? first, IList<Node>? second, int? start, int regret,
      float weight) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Add(instance.Move.ClosestTo(first.First()));

    second ??= new List<Node>();
    second.Add(instance.Move.FurthestTo(first.First()));
    second.Add(instance.Move.ClosestTo(second.First()));

    while (first.Count < instance.Dimension / 2) {
      var (previous, best) = FindFitsByRegretGain(instance, first, second, regret, weight);
      first.Insert(first.IndexOf(previous), best);

      (previous, best) = FindFitsByRegretGain(instance, second, first, regret, weight);
      second.Insert(second.IndexOf(previous), best);
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<IList<Node>> cycles, int regret, float weight) {
    cycles = cycles.ToArray();

    var points = instance.Move.FindFurthest(cycles.Count());
    foreach (var (path, point) in cycles.Zip(points)) path.Add(point);
    foreach (var path in cycles) instance.Move.ClosestTo(path.First(), cycles.Flatten());

    var count = cycles.Flatten().Count();
    while (true) {
      foreach (var path in cycles) {
        var (previous, best) = FindFitsByRegretGain(instance, path, cycles.Flatten(), regret, weight);
        path.Insert(path.IndexOf(previous), best);
        if (++count == instance.Dimension) return cycles;
      }
    }
  }

  private static (Node previous, Node best)
    FindFitsByRegretGain(this Instance instance, IList<Node> cycle, IEnumerable<Node> except, int regret, float weight)
    => instance
      .Nodes
      .Except(cycle)
      .Except(except)
      .Select(candidate =>
        cycle.Edges()
          .Select(edge => (edge.b, candidate, cost: instance.Gain.Insert(edge, candidate)))
          .OrderBy(n => n.cost)
          .ToList()
      )
      .OrderBy(match => CalculateRegret(match.Select(x => x.cost), regret) + weight * match.MinBy(x => x.cost).cost)
      .First()
      .MinBy(match => match.cost)
      .DropLast();

  private static int CalculateRegret(IEnumerable<int> costs, int regret) {
    var enumerable = costs as int[] ?? costs.ToArray();

    return enumerable.Skip(1).Take(regret - 1).Aggregate(0, (acc, a) => acc + (enumerable.First() - a));
  }
}
