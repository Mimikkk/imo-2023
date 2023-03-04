namespace Algorithms.DataStructures;

public record Instance(int Dimension, IReadOnlyList<Node> Nodes, int[,] Distances) {
  public static Instance Read(string path) {
    var nodes = Node.From(File.ReadLines($"{ProjectDirectory}/resources/instances/{path}.tsp")
        .Skip(6)
        .SkipLast(1))
      .ToList();

    return new Instance(nodes.Count, nodes, CreateDistanceMatrix(nodes));
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


  public int this[Node first, Node second] => Distances[first.Index, second.Index];

  public Node ClosestTo(Node node, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return Nodes.Except(except.Concat(Yield(node))).MinBy(n => this[node, n])!;
  }

  public Node FurthestTo(Node node, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return Nodes.Except(except.Concat(Yield(node))).MaxBy(n => this[node, n])!;
  }

  public int DistanceOf(IEnumerable<Node> cycle) {
    var items = cycle.ToArray();

    return this[items[0], items[^1]] + items.Pairwise().Sum(p => this[p.a, p.b]);
  }
}
