using imo_2023.DataStructures;

namespace imo_2023.Algorithms;

internal static class NearestNeighbourInstanceExtensions {
  public static IEnumerable<Node> PerformGreedyNearestNeighbour(this Instance instance) {
    var path = new List<Node> { Node.Choose(instance.Nodes) };

    while (path.Count < instance.Dimension)
      path.Add(instance.ClosestTo(path.Last(), path));

    return path;
  }
}
