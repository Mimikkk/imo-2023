using Algorithms.DataStructures;
using Algorithms.Extensions;
using static Algorithms.Algorithms.Algorithm;

namespace Algorithms.Algorithms;

public static class GreedyRegretCycleExpansionExtensions {
  private static (Node, Node) FindFitsByRegretGain(this Instance instance, IList<Node> cycle, IEnumerable<Node> except, int depth) => instance.Nodes
    .Except(cycle)
    .Except(except)
    .Select(candidate =>
      cycle.Edges()
        .Select(edge => (edge.b, candidate, cost: instance.InsertCost(edge, candidate)))
        .OrderBy(n => n.cost)
        .ToList()
    )
    .OrderBy(match => match.Skip(1).Take(depth - 1).Sum(p => match.First().cost - p.cost))
    .First()
    .MinBy(match => match.cost)
    .DropLast();

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansionWithKRegret(this Instance instance, int regret, IList<Node>? first = null, int? start = null) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Add(instance.ClosestTo(first.First()));

    while (first.Count < instance.Dimension) {
      var (previous, best) = FindFitsByRegretGain(instance, first, first, regret);
      first.Insert(first.IndexOf(previous), best);
    }

    return first;
  }

  public static (IEnumerable<Node>, IEnumerable<Node>)
    SearchWithGreedyCycleExpansionWithKRegret(this Instance instance, int regret, IList<Node>? first = null, IList<Node>? second = null, int? start = null) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Add(instance.ClosestTo(first.First()));

    second ??= new List<Node>();
    second.Add(instance.FurthestTo(first.First()));
    second.Add(instance.ClosestTo(second.First()));

    while (first.Count < instance.Dimension / 2) {
      var (previous, best) = FindFitsByRegretGain(instance, first, second, regret);
      first.Insert(first.IndexOf(previous), best);

      (previous, best) = FindFitsByRegretGain(instance, second, first, regret);
      second.Insert(second.IndexOf(previous), best);
    }

    return (first, second);
  }

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansionWith2Regret(this Instance instance, IList<Node>? cycle = null, int? start = null)
    => SearchWithGreedyCycleExpansionWithKRegret(instance, 2, cycle, start);

  public static (IEnumerable<Node>, IEnumerable<Node>)
    SearchWithGreedyCycleExpansionWith2Regret(this Instance instance, IList<Node>? first = null, IList<Node>? second = null, int? start = null)
    => SearchWithGreedyCycleExpansionWithKRegret(instance, 2, first, second, start);

  public static IEnumerable<IEnumerable<Node>>  Search(this Instance instance, SearchConfiguration configuration) {
    var paths = configuration.population.ToArray();
    var regret = configuration.regret!.Value;

    var points = instance.ChooseFurthest(paths.Length);
    foreach (var (path, point) in paths.Zip(points)) path.Add(point);
    foreach (var path in paths) instance.ClosestTo(path.First(), paths.Flatten());

    var count = paths.Flatten().Count();
    while (true) {
      foreach (var path in paths) {
        var (previous, best) = FindFitsByRegretGain(instance, path, paths.Flatten(), regret);
        path.Insert(path.IndexOf(previous), best);
        if (++count == instance.Dimension) return paths;
      }
    }
  }
}
