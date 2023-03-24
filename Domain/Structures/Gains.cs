using Domain.Extensions;

namespace Domain.Structures;

public sealed record Gains(Instance Instance) {
  public int Insert((Node a, Node b) edge, Node node) =>
    Instance[(edge.a, node, edge.b)] - Instance[edge];

  public int ExchangeVertex(IEnumerable<Node> cycle, Node a, Node b) {
    var enumerable = cycle as Node[] ?? cycle.ToArray();

    return ExchangeVertex(enumerable, enumerable, a, b);
  }

  public int ExchangeVertex(IEnumerable<Node> first, IEnumerable<Node> second, Node a, Node b) {
    var va = first.Neighbourhood(a);
    var vb = second.Neighbourhood(b);

    return Instance[va] + Instance[vb] - Instance[(va.a, vb.b, va.c)] - Instance[(vb.a, va.b, vb.c)];
  }

  public int ExchangeEdge(IEnumerable<Node> cycle, Node a, Node b) {
    var original = cycle as Node[] ?? cycle.ToArray();
    var result = original.ToList();
    Moves.ExchangeEdge(result, a, b);

    return Instance[result] - Instance[original];
  }
}
