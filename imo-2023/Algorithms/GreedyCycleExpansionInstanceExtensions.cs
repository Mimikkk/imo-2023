using imo_2023.DataStructures;

namespace imo_2023.Algorithms;

internal static class GreedyCycleExpansionInstanceExtensions {
  public static IEnumerable<Node> SearchWithGreedyCycleExpansion(this Instance instance) {
    var start = Node.Choose(instance.Nodes);
    var closest = instance.ClosestTo(start);

    var cycle = new List<Node> { start, closest };
    var distance = instance.DistanceOf(cycle);

    while (cycle.Count < instance.Dimension) {
      var distances = new List<(int i, int distance)>();

      foreach (var first in cycle) {
        foreach (var second in cycle.Except(Yield(first))) {
          foreach (var considered in instance.Nodes.Except(cycle)) {
            var newDistance = distance - instance[first, second] + instance[first, considered] + instance[considered, second];

            distances.Add((considered.Index, newDistance));
          }
        }
      }

      cycle.Add(instance.Nodes[distances.MinBy(d => d.distance).i]);
    }

    return cycle;
  }
}
