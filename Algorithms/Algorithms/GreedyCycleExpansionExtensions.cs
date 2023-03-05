using System.Diagnostics;
using Algorithms.DataStructures;
using Algorithms.Extensions;

namespace Algorithms.Algorithms;

public static class GreedyCycleExpansionExtensions {
  private static (Node previous, Node best) FindBestFitByLowestGain(this Instance instance, IList<Node> cycle, IEnumerable<Node> except) =>
    cycle.Edges()
      .SelectMany(p => instance.Nodes.Except(cycle)
        .Except(except)
        .Select(n => (p.b, n, distance: instance[n, p.a] + instance[n, p.b] - instance[p.a, p.b])))
      .MinBy(x => x.distance)
      .DropLast();

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansion(this Instance instance, IList<Node>? cycle = null, int? start = null) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension) {
      var (previous, best) = instance.FindBestFitByLowestGain(cycle, cycle);

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
      var (previous, best) = instance.FindBestFitByLowestGain(first, second);
      first.Insert(first.IndexOf(previous), best);

      (previous, best) = instance.FindBestFitByLowestGain(second, first);
      second.Insert(second.IndexOf(previous), best);
    }

    return (first, second);
  }
}
