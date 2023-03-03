namespace imo_2023.DataStructures;

internal record Node(int Index, int X, int Y) {
  private static readonly Random Random = new();

  public static IEnumerable<Node> From(IEnumerable<string> descriptors) =>
    descriptors
      .Select(descriptor => descriptor.Split(" ").Select(int.Parse).ToArray())
      .Select(coords => new Node(coords[0] - 1, coords[1], coords[2]));

  public int DistanceTo(Node other) => (int)Math.Round(Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2)));

  public static Node Choose(IEnumerable<Node> nodes) {
    var items = nodes.ToArray();

    return items[Random.Next(items.Length)];
  }
}
