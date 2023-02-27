using imo_2023.Extensions;

namespace imo_2023.DataStructures;

internal record Instance(int Dimension, IReadOnlyList<Node> Nodes, int[,] Distances) {
  public static Instance Read(string path) {
    var nodes = Node.From(File.ReadLines($"{ProjectDirectory}/resources/instances/{path}.tsp")
        .Skip(6)
        .SkipLast(1))
      .ToList();

    return new Instance(nodes.Count, nodes, CreateDistanceMatrix(nodes));
  }
  public static Instance KroA => Read("kroA100");
  public static Instance KroB => Read("kroB100");

  private static int[,] CreateDistanceMatrix(IEnumerable<Node> nodes) {
    var items = nodes.ToArray();
    var distances = new int[items.Length, items.Length];

    for (var i = 0; i < items.Length; ++i)
    for (var j = 0; j < items.Length; ++j)
      distances[i, j] = items[i].DistanceTo(items[j]);

    return distances;
  }


  public int this[Node first, Node second] => Distances[first.Index, second.Index];

  public Node ClosestTo(Node node, IEnumerable<Node>? except = null) {
    var closest = node;
    var excepted = except?.ToHashSet() ?? new HashSet<Node>();

    var distances = Distances.ReadRow(node.Index);
    for (var i = 0; Dimension > i; ++i)
      if (!excepted.Contains(Nodes[i]) && (closest.Index == node.Index || distances[i] < distances[closest.Index]))
        closest = Nodes[i];

    return closest;
  }

  public Node FurthestTo(Node node, IEnumerable<Node>? except = null) {
    var furthest = node;
    var excepted = except?.ToHashSet() ?? new HashSet<Node>();

    var distances = Distances.ReadRow(node.Index);
    for (var i = 0; Dimension > i; ++i)
      if (!excepted.Contains(Nodes[i]) && (furthest.Index == node.Index || distances[i] < distances[furthest.Index]))
        furthest = Nodes[i];

    return furthest;
  }

  public int DistanceOf(IEnumerable<Node> path) {
    var items = path.ToArray();
    var distance = 0;

    for (var i = 0; i < items.Length - 1; ++i) distance += Distances[items[i].Index, items[i + 1].Index];

    return distance;
  }
}
