using imo_2023.DataStructures;

namespace imo_2023.Algorithms;

internal static class NearestNeighbourInstanceExtensions {
  public static IEnumerable<Node> SearchWithGreedyNearestNeighbour(this Instance instance) {
    var path = new List<Node> { Node.Choose(instance.Nodes) };

    while (path.Count < instance.Dimension)
      path.Add(instance.ClosestTo(path.Last(), path));

    return path;
  }

  public static (IEnumerable<Node>, IEnumerable<Node>) SearchWithDoubleGreedyNearestNeighbour(this Instance instance) {
    var first = new List<Node> { Node.Choose(instance.Nodes) };
    var second = new List<Node> { instance.FurthestTo(first.First()) };

    while (first.Count < instance.Dimension / 2) {
      var closest = instance.ClosestTo(first.First(), first.Concat(second));
      var furthest = instance.ClosestTo(first.Last(), first.Concat(second));
      if (instance[closest, first.First()] > instance[first.Last(), furthest])
        first.Add(furthest);
      else first.Insert(0, closest);

      closest = instance.ClosestTo(second.First(), first.Concat(second));
      furthest = instance.ClosestTo(second.Last(), first.Concat(second));
      if (instance[closest, second.First()] > instance[second.Last(), furthest])
        second.Add(furthest);
      else second.Insert(0, closest);
    }

    return (first, second);
  }
}
