using Algorithms.DataStructures;
using Algorithms.Extensions;

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
    .OrderByDescending(match => match.Skip(1).Take(depth - 1).Sum(p => match.First().cost - p.cost))
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
}
