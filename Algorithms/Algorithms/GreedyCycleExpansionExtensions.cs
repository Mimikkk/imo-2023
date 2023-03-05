using System.Diagnostics;
using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyCycleExpansionExtensions {
  private static (Node, Node, int) FindBestFitToDistance(this Instance instance, IList<Node> cycle, int distance, IEnumerable<Node> except) => Enumerable
    .Range(1, cycle.Count - 1)
    .Select(x => (x, x - 1))
    .Concat(Yield((0, cycle.Count - 1)))
    .Select(p => (cycle[p.Item1], cycle[p.Item2], distance - instance[cycle[p.Item1], cycle[p.Item2]]))
    .SelectMany(p => instance.Nodes.Except(cycle)
      .Except(except)
      .Select(n => (p.Item1, n, p.Item3 + instance[n, p.Item1] + instance[n, p.Item2])))
    .MinBy(x => x.Item3);

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansion(this Instance instance, IList<Node>? cycle = null, int? start = null) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    var distance = instance.DistanceOf(cycle);
    while (cycle.Count < instance.Dimension) {
      (var previous, var best, distance) = instance.FindBestFitToDistance(cycle, distance, cycle);

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

    var firstDistance = instance.DistanceOf(first);
    var secondDistance = instance.DistanceOf(second);
    while (first.Count < instance.Dimension / 2) {
      (var previous, var best, firstDistance) = instance.FindBestFitToDistance(first, firstDistance, second);
      first.Insert(first.IndexOf(previous), best);

      (previous, best, secondDistance) = instance.FindBestFitToDistance(second, secondDistance, first);
      second.Insert(second.IndexOf(previous), best);
    }

    return (first, second);
  }
}
