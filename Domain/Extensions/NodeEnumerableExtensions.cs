using Domain.Structures;

namespace Domain.Extensions;

public static class NodeEnumerableExtensions {
  public static IEnumerable<(Node a, Node b)> Edges(this IEnumerable<Node> cycle) {
    var items = cycle.ToArray();

    return items.Length == 0
      ? Array.Empty<(Node a, Node b)>()
      : items.Pairwise().Concat(Yield((items.Last(), items.First())));
  }

  private enum GeometricRelation : byte {
    LeftOf,
    RightOf,
    Collinear
  }

  private static GeometricRelation CalculateRelation(Node a, Node b, Node c) =>
    ((float)(a.X - c.X) * (b.Y - c.Y) - (a.Y - c.Y) * (b.X - c.X)) switch {
      < 0.00001f and > -0.00001f => GeometricRelation.Collinear,
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
