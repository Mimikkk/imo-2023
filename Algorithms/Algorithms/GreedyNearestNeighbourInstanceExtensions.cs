using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyNearestNeighbourExtensions {
  public static IEnumerable<Node>
    SearchWithGreedyNearestNeighbour(this Instance instance, IList<Node>? path = null) {
    path ??= new List<Node>();

    path.Add(Node.Choose(instance.Nodes));
    while (path.Count < instance.Dimension) path.Add(instance.ClosestTo(path.Last(), path));

    return path;
  }

  public static (IEnumerable<Node>, IEnumerable<Node>)
    SearchDoubleWithGreedyNearestNeighbour(this Instance instance, IList<Node>? first = null, IList<Node>? second = null) {
    first ??= new List<Node>();
    second ??= new List<Node>();
    first.Add(Node.Choose(instance.Nodes));
    second.Add(instance.FurthestTo(first.First()));

    while (first.Count < instance.Dimension / 2) {
      var closestToTail = instance.ClosestTo(first.First(), first.Concat(second));
      var closestToHead = instance.ClosestTo(first.Last(), first.Concat(second));
      if (instance[closestToTail, first.First()] > instance[first.Last(), closestToHead]) first.Add(closestToHead);
      else first.Insert(0, closestToTail);

      closestToTail = instance.ClosestTo(second.First(), first.Concat(second));
      closestToHead = instance.ClosestTo(second.Last(), first.Concat(second));
      if (instance[closestToTail, second.First()] > instance[second.Last(), closestToHead]) second.Add(closestToHead);
      else second.Insert(0, closestToTail);
    }

    return (first, second);
  }
}
