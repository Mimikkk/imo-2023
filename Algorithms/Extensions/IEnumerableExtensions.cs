using Algorithms.DataStructures;

namespace Algorithms.Extensions;

public static class IEnumerableExtensions {
  public static IEnumerable<T> Yield<T>(T item) { yield return item; }
  public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items) => items.SelectMany(x => x);

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
    if (count > enumerable.Length)
      yield return enumerable;

    foreach (var (element, i) in enumerable.Select((item, index) => (item, index + 1))) {
      foreach (var result in Combinations(enumerable.Skip(i), count - 1))
        yield return Yield(element).Concat(result);
    }
  }
  public static IEnumerable<(Node a, Node b)> Edges(this IEnumerable<Node> cycle) {
    var items = cycle.ToArray();

    return items.Pairwise().Concat(Yield((items.Last(), items.First())));
  }

  private enum GeometricRelation : byte { LeftOf, RightOf, Collinear }

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
