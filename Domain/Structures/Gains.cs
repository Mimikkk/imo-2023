namespace Domain.Structures;

public sealed record Gains(Instance Instance) {
  public int Insert((Node a, Node b) edge, Node node) =>
    Instance[(edge.a, node, edge.b)] - Instance[edge];

  public int ExchangeVertex(IEnumerable<Node> cycle, Node a, Node b) {
    var original = cycle as Node[] ?? cycle.ToArray();
    var result = original.ToList();
    Moves.ExchangeVertex(result, a, b);

    return Instance[original] - Instance[result];
  }

  public int ExchangeVertex(IEnumerable<Node> first, IEnumerable<Node> second, Node a, Node b) {
    var originalf = first as Node[] ?? first.ToArray();
    var originals = second as Node[] ?? second.ToArray();
    var resultf = originalf.ToList();
    var results = originals.ToList();
    Moves.ExchangeVertex(resultf, results, a, b);

    return Instance[originalf] - Instance[resultf] + Instance[originals] - Instance[results];
  }

  public int ExchangeEdge(IEnumerable<Node> cycle, Node a, Node b) {
    var original = cycle as Node[] ?? cycle.ToArray();
    var result = original.ToList();
    Moves.ExchangeEdge(result, a, b);

    return Instance[original] - Instance[result];
  }
}
