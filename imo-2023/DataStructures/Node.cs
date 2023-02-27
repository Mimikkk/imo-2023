namespace imo_2023.DataStructures;

internal record Node(int X, int Y)
{
  public static IEnumerable<Node> From(IEnumerable<string> descriptors) =>
    descriptors.Select(descriptor => descriptor.Split(" ")).Select(coords => new Node(
      int.Parse(coords[1]), int.Parse(coords[2])
    ));

  public int DistanceTo(Node other) => (int)Math.Round(Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2)));
}
