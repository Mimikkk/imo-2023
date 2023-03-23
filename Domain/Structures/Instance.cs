using Domain.Extensions;

namespace Domain.Structures;

public sealed record Instance {
  public static Instance Read(string name) {
    var nodes = Node.From(File.ReadLines(Path.Combine(InstanceDirectory, $"{name}.tsp"))
        .Skip(6)
        .SkipLast(1))
      .ToList();

    return new Instance(nodes.Count, nodes, name);
  }

  private static int[,] CreateDistanceMatrix(IEnumerable<Node> nodes) {
    var items = nodes.ToArray();
    var distances = new int[items.Length, items.Length];

    for (var i = 0; i < items.Length; ++i)
    for (var j = 0; j < items.Length; ++j) {
      if (i == j) continue;
      distances[i, j] = items[i].DistanceTo(items[j]);
      distances[j, i] = items[j].DistanceTo(items[i]);
    }

    return distances;
  }

  public int this[Node first, Node second] => _distances[first.Index, second.Index];
  public int this[int first, int second] => _distances[first, second];
  public int this[(Node a, Node b) edge] => this[edge.a, edge.b];
  public int this[(Node a, Node b, Node c) vertex] => this[vertex.a, vertex.b] + this[vertex.b, vertex.c];
  public int this[IEnumerable<Node> cycle] => cycle.Edges().Sum(edge => this[edge]);

  public readonly Moves Move;
  public readonly Gains Gain;
  public readonly int Dimension;
  public readonly List<Node> Nodes;
  public readonly string Name;

  private readonly int[,] _distances;

  private Instance(int dimension, List<Node> nodes, string name) {
    Dimension = dimension;
    Nodes = nodes;
    _distances = CreateDistanceMatrix(nodes);
    Name = name;
    Gain = new(this);
    Move = new(this);
  }

  private static readonly string InstanceDirectory = Path.Combine(ResourcesDirectory, "Instances");
}
