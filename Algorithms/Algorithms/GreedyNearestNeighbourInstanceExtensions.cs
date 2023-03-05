using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyNearestNeighbourExtensions {
  private static void AppendClosestHeadOrTail(this Instance instance, IList<Node> path, IEnumerable<Node> except) {
    var excepted = except.ToArray();
    var closestToTail = instance.ClosestTo(path.First(), excepted);
    var closestToHead = instance.ClosestTo(path.Last(), excepted);

    if (instance[closestToTail, path.First()] > instance[path.Last(), closestToHead]) path.Add(closestToHead);
    else path.Insert(0, closestToTail);
  }

  public static IEnumerable<Node>
    SearchWithGreedyNearestNeighbour(this Instance instance, IList<Node>? path = null) {
    path ??= new List<Node>();

    path.Add(Node.Choose(instance.Nodes));
    while (path.Count < instance.Dimension) instance.AppendClosestHeadOrTail(path, path);

    return path;
  }

  public static (IEnumerable<Node>, IEnumerable<Node>)
    SearchWithGreedyNearestNeighbour(this Instance instance, IList<Node>? first = null, IList<Node>? second = null) {
    first ??= new List<Node>();
    second ??= new List<Node>();
    first.Add(Node.Choose(instance.Nodes));
    second.Add(instance.FurthestTo(first.First()));

    while (first.Count < instance.Dimension / 2) {
      instance.AppendClosestHeadOrTail(first, first.Concat(second));
      instance.AppendClosestHeadOrTail(second, first.Concat(second));
    }

    return (first, second);
  }
}
