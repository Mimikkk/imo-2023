using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyCycleExpansionExtensions {
  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansion(this Instance instance, IList<Node>? cycle = null) {
    cycle ??= new List<Node>();
    cycle.Add(Node.Choose(instance.Nodes));
    cycle.Add(instance.ClosestTo(cycle.First()));

    var distance = instance.DistanceOf(cycle);
    while (cycle.Count < instance.Dimension) {
      List<(Node previous, Node next, int distance)> distances = new();
      foreach (var first in cycle) {
        foreach (var second in cycle.Except(Yield(first))) {
          foreach (var considered in instance.Nodes.Except(cycle)) {
            var dist = distance - instance[first, second] + instance[first, considered] + instance[second, considered];
            distances.Add((first, considered, dist));
          }
        }
      }

      var (previous, next, bestDistance) = distances.MinBy(x => x.distance);


      // var (previous, next, bestDistance) = (
      //   from first in cycle
      //   from second in cycle.Except(Yield(first))
      //   from considered in instance.Nodes.Except(cycle)
      //   let dist = distance - instance[first, second] + instance[first, considered] + instance[second, considered]
      //   select (first, considered, dist)).MinBy(d => d.dist);

      distance = bestDistance;
      cycle.Insert(cycle.IndexOf(previous) -1, next);
    }

    return cycle;
  }
}
