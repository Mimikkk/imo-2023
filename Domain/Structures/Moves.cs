using Domain.Extensions;

namespace Domain.Structures;

public sealed record Moves {
  public void Insert(IList<Node> cycle, Node node, (Node a, Node b) edge) {
    if (edge.a == cycle.First() && edge.b == cycle.Last() || edge.b == cycle.First() && edge.a == cycle.Last()) {
      cycle.Add(node);
      return;
    }


    var ia = cycle.IndexOf(edge.a);
    var ib = cycle.IndexOf(edge.b);
    cycle.Insert(ia > ib ? ia : ib, node);
  }

  public void ExchangeVertex(IList<Node> first, IList<Node> second, Node a, Node b) {
    var (ia, ib) = (first.IndexOf(a), second.IndexOf(b));
    first.Remove(a);
    first.Insert(ia, b);
    second.Remove(b);
    second.Insert(ib, a);
  }

  public void ExchangeVertex(IList<Node> cycle, Node a, Node b) => cycle.Swap(a, b);

  public void ExchangeEdge(IList<Node> cycle, Node a, Node b) {
    var ia = cycle.IndexOf(a);
    var ib = cycle.IndexOf(b);
    if (ia > ib) (ia, ib) = (ib, ia);
    if (ia == 0 && ib == cycle.Count - 1) cycle.Swap(ia, ib);

    var elementCount = ib - ia + 1;
    for (var i = 0; i < elementCount / 2; ++i) {
      cycle.Swap((ia + cycle.Count + i) % cycle.Count, (ib + cycle.Count - i) % cycle.Count);
    }
  }
}
