using imo_2023.DataStructures;

namespace imo_2023.Algorithms;

internal static class NearestNeighbourInstanceExtensions
{
  public static void PerformGreedyNearestNeighbour(this Instance instance)
  {
    var chosen = Node.Choose(instance.Nodes);
    var path = new List<Node> { chosen };

    path.First().ClosestTo(instance);
  }
}
