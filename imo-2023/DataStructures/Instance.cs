namespace imo_2023.DataStructures;

internal record Instance(int Dimension, IReadOnlyList<Node> Nodes, int[,] Distances)
{
  public static Instance Read(string path)
  {
    var nodes = Node.From(File.ReadLines($"{Prelude.ProjectDirectory}/resources/instances/{path}.tsp")
      .Skip(6).SkipLast(1)).ToList();

    return new Instance(nodes.Count, nodes, CreateDistanceMatrix(nodes));
  }

  private static int[,] CreateDistanceMatrix(IEnumerable<Node> nodes)
  {
    var items = nodes.ToArray();
    var matrix = new int[items.Length, items.Length];

    for (var i = 0; i < items.Length; i++)
    for (var j = 0; j < items.Length; j++)
      matrix[i, j] = items[i].DistanceTo(items[j]);

    return matrix;
  }
}
