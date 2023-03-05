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
      List<(Node first, Node second, Node considered, int distance)> distances = new();

      foreach (var _first in cycle) {
        foreach (var _second in cycle.Except(Yield(_first))) {
          foreach (var _considered in instance.Nodes.Except(cycle)) {
            var _distance = distance - instance[_first, _second] + instance[_first, _considered] + instance[_second, _considered];

            distances.Add((_first, _second, _considered, _distance));
          }
        }
      }

      var (first, second, considered, _) = distances.MinBy(x => x.distance);

      // var (previous, next, bestDistance) = (
      //   from first in cycle
      //   from second in cycle.Except(Yield(first))
      //   from considered in instance.Nodes.Except(cycle)
      //   let dist = distance - instance[first, second] + instance[first, considered] + instance[second, considered]
      //   select (first, considered, dist)).MinBy(d => d.dist);

      var c1 = cycle.ToList();
      c1.Insert(c1.IndexOf(first), considered);
      var c2 = cycle.ToList();
      c2.Insert(c2.IndexOf(second), considered);
      var c3 = cycle.ToList();
      c3.Insert(c3.IndexOf(first) + 1, considered);
      var c4 = cycle.ToList();
      c4.Insert(c4.IndexOf(second) + 1, considered);

      var min = new List<List<Node>> { c1, c2, c3, c4 }.MinBy(instance.DistanceOf)!;

      Debug.Write("elements: ");
      Debug.WriteLine(string.Join(", ", new List<Node> { first, second, considered }.Select(x => x.Index)));
      Debug.Write("c1 indices: ");
      Debug.WriteLine(string.Join(", ", new List<Node> { first, second, considered }.Select(x => c1.IndexOf(x))));
      Debug.Write("c2 indices: ");
      Debug.WriteLine(string.Join(", ", new List<Node> { first, second, considered }.Select(x => c2.IndexOf(x))));
      Debug.Write("c3 indices: ");
      Debug.WriteLine(string.Join(", ", new List<Node> { first, second, considered }.Select(x => c3.IndexOf(x))));
      Debug.Write("c4 indices: ");
      Debug.WriteLine(string.Join(", ", new List<Node> { first, second, considered }.Select(x => c4.IndexOf(x))));

      if (min == c1) {
        cycle.Insert(cycle.IndexOf(first), considered);
      } else if (min == c2) {
        Debug.WriteLine("c2");
        cycle.Insert(cycle.IndexOf(second), considered);
      } else if (min == c3) {
        Debug.WriteLine("c3");
        cycle.Insert(cycle.IndexOf(first) + 1, considered);
      } else if (min == c4) {
        Debug.WriteLine("c4");
        cycle.Insert(cycle.IndexOf(second) + 1, considered);
      }
      distance = instance.DistanceOf(cycle);
    }

    return cycle;
  }
}
