using imo_2023.DataStructures;

namespace imo_2023.Algorithms;

internal static class NearestNeighbourInstanceExtensions {
  public static void PerformGreedyNearestNeighbour(this Instance instance) {
    var path = new List<Node> { Node.Choose(instance.Nodes) };
    path.Add(instance.ClosestTo(path.Last(), path));

    while (path.Count < instance.Dimension) path.Add(instance.ClosestTo(path.Last(), path));

    Wl(new[] { path.ToHashSet().Count, path.Count });
  }
}
