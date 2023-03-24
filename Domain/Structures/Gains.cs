using System.Diagnostics;
using System.Net.NetworkInformation;
using Domain.Extensions;

namespace Domain.Structures;

public sealed record Gains(Instance Instance) {
  public int Insert((Node a, Node b) edge, Node node) =>
    Instance[(edge.a, node, edge.b)] - Instance[edge];

  public int ReplaceVertex(IEnumerable<Node> cycle, Node a, Node b) {
    var va = cycle.Neighbourhood(a);

    return Instance[va] - Instance[va with { b = b }];
  }

  public int ExchangeVertex(IEnumerable<Node> cycle, Node na, Node nb) {
    var path = cycle.ToList();
    var i = path.IndexOf(na);
    var j = path.IndexOf(nb);
    if (i > j) (i, j) = (j, i);

    var (a, b, c) = path.Neighbourhood(path[i]);
    var (d, e, f) = path.Neighbourhood(path[j]);

    if (j - i == 1)
      return Instance[a, b] + Instance[e, f] - Instance[a, e] - Instance[b, f];

    if ((i, j) == (0, path.Count - 1))
      return Instance[b, c] + Instance[d, e] - Instance[e, c] - Instance[d, b];

    return Instance[(a, b, c)] + Instance[(d, e, f)] - Instance[(a, e, c)] - Instance[(d, b, f)];
  }

  public int ExchangeVertex(IEnumerable<Node> first, IEnumerable<Node> second, Node a, Node b) =>
    ReplaceVertex(first, a, b) + ReplaceVertex(second, b, a);

  public int ExchangeEdge(IEnumerable<Node> cycle, Node na, Node nb) {
    var path = cycle.ToList();
    var i = path.IndexOf(na);
    var j = path.IndexOf(nb);
    if (i > j) (i, j) = (j, i);

    var (a, b, c, d) = i == 0 && j == path.Count - 1
      ? (path[i], path.NextTo(path[i]), path.PreviousTo(path[j]), path[j])
      : (path.PreviousTo(path[i]), path[i], path[j], path.NextTo(path[j]));

    return Instance[a, b] + Instance[c, d] - Instance[a, c] - Instance[b, d];
  }
}
