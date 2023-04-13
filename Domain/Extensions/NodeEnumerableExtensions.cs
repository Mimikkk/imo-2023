using Domain.Structures;

namespace Domain.Extensions;

public static class NodeEnumerableExtensions {
  public static IEnumerable<(Node a, Node b)> Edges(this IEnumerable<Node> cycle) {
    var nodes = cycle.ToArray();

    for (var i = 0; i < nodes.Length; ++i) {
      yield return (
        nodes[i % nodes.Length],
        nodes[(i + 1) % nodes.Length]
      );
    }
  }

  public static IEnumerable<(Node a, Node b, Node c)> Vertices(this IEnumerable<Node> cycle) {
    var nodes = cycle.ToArray();

    for (var i = 0; i < nodes.Length; ++i) {
      yield return (
        nodes[i % nodes.Length],
        nodes[(i + 1) % nodes.Length],
        nodes[(i + 2) % nodes.Length]
      );
    }
  }

  public static (Node a, Node b, Node c) Neighbourhood(this IEnumerable<Node> cycle, Node at) {
    var nodes = cycle.ToList();
    return (nodes.PreviousTo(at), at, nodes.NextTo(at));
  }

  public static bool IsNextTo(this IEnumerable<Node> cycle, Node a, Node b) {
    var enumerable = cycle as Node[] ?? cycle.ToArray();
    if (!enumerable.Contains(a) && enumerable.Contains(b)) return false;
    var (previous, _, next) = enumerable.Neighbourhood(a);
    return Yield(previous, next).Contains(b);
  }


  public enum Orientation { Before, After }
  public static Orientation? OrientationOf(IList<Node> cycle, Node a, Node b) {
    if (cycle.IsBefore(a, b)) return Orientation.Before;
    if (cycle.IsBefore(b, a)) return Orientation.After;
    return null;
  }
  public static Orientation? OrientationOf(IList<Node> cycle, (Node a, Node b) edge) => OrientationOf(cycle, edge.a, edge.b);

  public static bool IsBefore(this IEnumerable<Node> cycle, Node a, Node b) {
    var enumerable = cycle as Node[] ?? cycle.ToArray();
    if (!enumerable.Contains(a) && enumerable.Contains(b)) return false;
    var (previous, _, _) = enumerable.Neighbourhood(a);

    return previous == b;
  }
  public static bool IsAfter(this IEnumerable<Node> cycle, Node a, Node b) {
    var enumerable = cycle as Node[] ?? cycle.ToArray();
    if (!enumerable.Contains(a) && enumerable.Contains(b)) return false;
    var (_, _, next) = enumerable.Neighbourhood(a);

    return next == b;
  }

  public static Node PreviousTo(this IEnumerable<Node> cycle, Node node) {
    var nodes = cycle.ToList();

    return nodes[(nodes.IndexOf(node) + nodes.Count - 1) % nodes.Count];
  }

  public static Node NextTo(this IEnumerable<Node> cycle, Node node) {
    var nodes = cycle.ToList();

    return nodes[(nodes.IndexOf(node) + 1) % nodes.Count];
  }

  private enum GeometricRelation : byte {
    LeftOf,
    RightOf,
    Collinear
  }

  private static GeometricRelation CalculateRelation(Node a, Node b, Node c) =>
    ((float)(a.X - c.X) * (b.Y - c.Y) - (a.Y - c.Y) * (b.X - c.X)) switch {
      < 0.00001f and > -0.00001f => GeometricRelation.Collinear,
      < 0                        => GeometricRelation.RightOf,
      _                          => GeometricRelation.LeftOf
    };

  public static IEnumerable<Node> Hull(this IEnumerable<Node> nodes) {
    var points = nodes.ToList();
    var hull = new List<Node> { points.MinBy(p => p.X)! };
    points.Remove(hull.First());

    var (collinear, counter) = (new List<Node>(), 0);
    while (hull.Count < 2 || hull.First() != hull.Last()) {
      if (++counter == 3) points.Add(hull.First());

      var next = points.First();
      foreach (var node in points.Skip(1))
        switch (CalculateRelation(hull.Last(), next, node)) {
          case GeometricRelation.Collinear:
            collinear.Add(node);
            break;
          case GeometricRelation.RightOf:
            next = node;
            collinear.Clear();
            break;
        }


      if (collinear.Count > 0) {
        collinear.Add(next);

        collinear = collinear
          .OrderBy(n => (n - hull.Last()).SquareMagnitude)
          .ToList();

        hull.AddRange(collinear);
        points.RemoveAll(collinear.Contains);
        collinear.Clear();
        continue;
      }

      hull.Add(next);
      points.Remove(next);
    }

    return hull;
  }
}
