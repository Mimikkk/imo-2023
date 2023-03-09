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
public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> items, int count) {
    var enumerable = items as T[] ?? items.ToArray();
    if (count == 1)
      foreach (var item in enumerable)
        yield return Yield(item);

    foreach (var (element, i) in enumerable.Select((item, index) => (item, index + 1))) {
      foreach (var result in Combinations(enumerable.Skip(i), count - 1))
        yield return Yield(element).Concat(result);
    }
  }
  public static IEnumerable<(Node a, Node b)> Edges(this IEnumerable<Node> cycle) {
    var items = cycle.ToArray();

    return items.Pairwise().Concat(Yield((items.Last(), items.First())));
  }

  private enum GeometricRelation {
    LeftOf,
    RightOf,
    OnTheLine
  }

  private static GeometricRelation CalculateRelation(Node a, Node b, Node c) =>
    ((float)(a.X - c.X) * (b.Y - c.Y) - (a.Y - c.Y) * (b.X - c.X)) switch {
      < 0.00001f and > -0.00001f => GeometricRelation.OnTheLine,
      < 0 => GeometricRelation.RightOf,
      _ => GeometricRelation.LeftOf
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
          case GeometricRelation.OnTheLine:
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

    return hull;}


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
