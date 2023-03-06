using Algorithms.DataStructures;

namespace Algorithms.Extensions;

public static class IEnumerableExtensions {
  public static IEnumerable<T> Yield<T>(T item) {
    yield return item;
  }

  public static IEnumerable<(T a, T b)> Pairwise<T>(this IEnumerable<T> source) {
    var previous = default(T);
    using var it = source.GetEnumerator();

    if (it.MoveNext()) previous = it.Current;
    while (it.MoveNext()) yield return (previous!, previous = it.Current);
  }

  public static IEnumerable<(Node a, Node b)> Edges(this IEnumerable<Node> cycle) {
    var items = cycle.ToArray();

    return items.Pairwise().Concat(Yield((items[0], items[^1])));
  }


  public static List<Node> Hull(this IEnumerable<Node> nodes) {
    nodes = nodes.ToList();
    if (nodes.Count() <= 3) return nodes.ToList();

    var hull = new List<Node> { nodes.MinBy(n => n.X)! };
    var points = nodes.Except(hull).ToList();

    var current = hull.First();
    var counter = 0;
    var collinear = new List<Node>();
    while (true) {
      if (counter == 2) points.Add(hull[0]);
      var next = Node.Choose(points);

      var (a, b) = (current, next);
      foreach (var node in points.Except(Yield(next)))
        switch (Ccw(a, b, node)) {
          case GeometryRelation.Collinear:
            collinear.Add(node);
            break;
          case GeometryRelation.RightOf:
            (next, b) = (node, node);
            collinear.Clear();
            break;
        }

      if (next.Equals(hull.First())) return hull;

      if (collinear.Count > 0) {
        collinear.Add(next);

        collinear = collinear
          .Select(n => n - current)
          .OrderBy(n => n.SquareMagnitude)
          .ToList();

        hull.AddRange(collinear);
        current = collinear.Last();
        points.RemoveAll(collinear.Contains);
        collinear.Clear();
      } else {
        hull.Add(next);
        points.Remove(next);
        current = next;
      }

      counter += 1;
    }
  }

  private enum GeometryRelation : byte { LeftOf, RightOf, Collinear }
  private static GeometryRelation Ccw(Node a, Node b, Node p) =>
    (float)((a.X - p.X) * (b.Y - p.Y) - (a.Y - p.Y) * (b.X - p.X)) switch {
      < 0.00001f and > -0.00001f => GeometryRelation.Collinear,
      < 0f                       => GeometryRelation.RightOf,
      _                          => GeometryRelation.LeftOf
    };
}
