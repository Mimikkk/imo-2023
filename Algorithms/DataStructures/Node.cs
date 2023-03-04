using ScottPlot;

namespace Algorithms.DataStructures;

public record Node(int Index, int X, int Y) {
  private static readonly Random Random = new();

  public static IEnumerable<Node> From(IEnumerable<string> descriptors) =>
    descriptors
      .Select(descriptor => descriptor.Split(" ").Select(int.Parse).ToArray())
      .Select(coords => new Node(coords[0] - 1, coords[1], coords[2]));

  public int DistanceTo(Node other) => (int)Math.Round(Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2)));
  public int DistanceTo(Coordinates other) => (int)Math.Round(Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2)));

  public static Node Choose(IEnumerable<Node> nodes) {
    var items = nodes.ToArray();

    return items[Random.Next(items.Length)];
  }

  public static implicit operator (int X, int Y)(Node node) => (node.X, node.Y);
  public void Deconstruct(out int x, out int y) {
    x = X;
    y = Y;
  }
  public void Deconstruct(out int index, out int x, out int y) {
    index = Index;
    (x, y) = this;
  }

  public bool InBounds(double minx, double maxx, double miny, double maxy) {
    var (x, y) = this;
    return x >= minx && x < maxx && y >= miny && y < maxy;
  }
}
