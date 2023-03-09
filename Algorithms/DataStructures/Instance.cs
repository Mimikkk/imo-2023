using System.Diagnostics;
using System.Numerics;

namespace Algorithms.DataStructures;

public record Instance(int Dimension, List<Node> Nodes, int[,] Distances) {
  public static Instance Read(string name) {
    var nodes = Node.From(File.ReadLines($"{ProjectDirectory}/Resources/Instances/{name}.tsp")
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
  public int this[int first, int second] => Distances[first, second];
  public int this[(Node a, Node b) edge] => this[edge.a, edge.b];

  public Node ClosestTo(Node node, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return Nodes.Except(except.Concat(Yield(node))).MinBy(n => this[n, node])!;
  }

  public Node FurthestTo(Node node, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return Nodes.Except(except.Concat(Yield(node))).MaxBy(n => this[n, node])!;
  }

  public IEnumerable<Node> ChooseFurthest(int count) =>
    Nodes.Hull().Combinations(count).MaxBy(nodes => nodes.Edges().Sum(edge => this[edge]))!;

  public int DistanceOf(IEnumerable<Node> cycle) =>
    cycle.Edges().Sum(edge => this[edge]);

  public int InsertCost((Node a, Node b) edge, Node node) =>
    this[edge.a, node] + this[node, edge.b] - this[edge];
}
