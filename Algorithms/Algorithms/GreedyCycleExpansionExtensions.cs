using System.Diagnostics;
using Algorithms.DataStructures;
using Algorithms.Extensions;

namespace Algorithms.Algorithms;

public static class GreedyCycleExpansionExtensions {
  private static (Node previous, Node best) FindBestFitByLowestGain(this Instance instance, IList<Node> cycle,
    IEnumerable<Node> except) =>
    cycle.Edges()
      .SelectMany(p => instance.Nodes.Except(cycle)
        .Except(except)
        .Select(n => (p.b, n, cost: instance.InsertCost(p, n))))
      .MinBy(x => x.cost)
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
    SearchWithGreedyCycleExpansion(this Instance instance, IList<Node>? first = null, IList<Node>? second = null,
      int? start = null) {
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

  public static IEnumerable<IEnumerable<Node>>
    SearchWithGreedyCycleExpansion(this Instance instance, IEnumerable<IList<Node>> cycles, int? start = null) {
    var hull = instance.Nodes.Hull();
    var count = 5;
    var nodes = new List<Node> { Node.Choose(hull) };
    var maximal = 0;
    while (nodes.Count < count) {
    }

    return new List<IEnumerable<Node>>();
  }
}
