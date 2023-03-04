using System.Collections.ObjectModel;
using imo_2023.DataStructures;

namespace imo_2023.Algorithms;

public static class GreedyCycleExpansionInstanceExtensions {
  public static IEnumerable<Node> SearchWithGreedyCycleExpansion(this Instance instance) {
    var start = Node.Choose(instance.Nodes);
    var closest = instance.ClosestTo(start);

    var cycle = new List<Node> { start, closest };
    var distance = instance.DistanceOf(cycle);
    while (cycle.Count < instance.Dimension) {
      var (previous, next, bestDistance) = (
        from first in cycle
        from second in cycle.Except(Yield(first))
        from considered in instance.Nodes.Except(cycle)
        let dist = distance - instance[first, second] + instance[first, considered] + instance[considered, second]
        select (first, considered, dist)).MinBy(d => d.dist);

      distance = bestDistance;
      cycle.Insert(cycle.IndexOf(previous), next);
    }

    return cycle;
  }
}
