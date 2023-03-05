using System.Diagnostics;
using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyCycleExpansionExtensions {
  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansion(this Instance instance, IList<Node>? cycle = null, int? start = null) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    var distance = instance.DistanceOf(cycle);
    while (cycle.Count < instance.Dimension) {
      (var previous, var best, distance) = Enumerable
        .Range(1, cycle.Count - 1)
        .Select(x => (x, x - 1))
        .Concat(Yield((0, cycle.Count - 1)))
        .Select(p => (cycle[p.Item1], cycle[p.Item2], distance - instance[cycle[p.Item1], cycle[p.Item2]]))
        .SelectMany(p => instance.Nodes.Except(cycle).Select(n => (p.Item1, n, p.Item3 + instance[n, p.Item1] + instance[n, p.Item2])))
        .MinBy(x => x.Item3);

      cycle.Insert(cycle.IndexOf(previous), best);
    }

    return cycle;
  }
}
