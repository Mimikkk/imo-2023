using System.Diagnostics;
using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyCycleExpansionExtensions {
  private static (Node, Node, int) FindBestFitByLowestGain(this Instance instance, IList<Node> cycle, IEnumerable<Node> except) => Enumerable
    .Range(1, cycle.Count - 1)
    .Select(x => (x, x - 1))
    .Concat(Yield((0, cycle.Count - 1)))
    .Select(p => (cycle[p.Item1], cycle[p.Item2]))
    .SelectMany(p => instance.Nodes.Except(cycle)
      .Except(except)
      .Select(n => (p.Item1, n, instance[n, p.Item1] + instance[n, p.Item2] - instance[p.Item1, p.Item2])))
    .MinBy(x => x.Item3);

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansion(this Instance instance, IList<Node>? cycle = null, int? start = null) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension) {
      var (previous, best, _) = instance.FindBestFitByLowestGain(cycle, cycle);

      cycle.Insert(cycle.IndexOf(previous), best);
    }

    return cycle;
  }
  public static (IEnumerable<Node>, IEnumerable<Node>)
    SearchDoubleWithGreedyCycleExpansion(this Instance instance, IList<Node>? first = null, IList<Node>? second = null, int? start = null) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Add(instance.ClosestTo(first.First()));

    second ??= new List<Node>();
    second.Add(instance.FurthestTo(first.First()));
    second.Add(instance.ClosestTo(second.First()));

    while (first.Count < instance.Dimension / 2) {
      var (previous, best, _) = instance.FindBestFitByLowestGain(first, second);
      first.Insert(first.IndexOf(previous), best);

      (previous, best, _) = instance.FindBestFitByLowestGain(second, first);
      second.Insert(second.IndexOf(previous), best);
    }

    return (first, second);
  }
}
