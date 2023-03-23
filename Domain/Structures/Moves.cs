using Domain.Extensions;

namespace Domain.Structures;

public sealed record Moves(Instance Instance) {
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

  public Node ClosestTo(Node node, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return Instance.Nodes.Except(except.Concat(Yield(node))).MinBy(n => Instance[n, node])!;
  }

  public Node FurthestTo(Node node, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return Instance.Nodes.Except(except.Concat(Yield(node))).MaxBy(n => Instance[n, node])!;
  }

  public IEnumerable<Node> FindFurthest(int count, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return count < 2
      ? Yield(Node.Choose(Instance.Nodes)).Except(except)
      : Instance.Nodes.Hull().Except(except).Combinations(count)
        .MaxBy(nodes => nodes.Edges().Sum(edge => Instance[edge]))!;
  }

  public ((Node a, Node b), int) FindBestInsertion(IList<Node> cycle, Node node) {
    var edge = (cycle.First(), cycle.Last());
    var gain = Instance.Gain.Insert(edge, node);

    foreach (var (a, b) in cycle.Edges()) {
      var current = Instance.Gain.Insert((a, b), node);
      if (current <= gain) continue;
      (gain, edge) = (current, (a, b));
    }

    return (edge, gain);
  }

  public (Node a, Node b) ClosestToHeadOrTail(IList<Node> path, IEnumerable<Node> except) {
    var excepted = except.ToArray();
    var tail = ClosestTo(path.First(), excepted);
    var head = ClosestTo(path.Last(), excepted);

    return (head, tail);
  }

  public void AppendClosestToHeadOrTail(IList<Node> path, IEnumerable<Node> except) {
    var (head, tail) = ClosestToHeadOrTail(path, except);

    if (Instance[tail, path.First()] > Instance[path.Last(), head]) path.Add(head);
    else path.Insert(0, tail);
  }


  public (Node previous, Node best, int gain)
    FindBestFitByLowestGain(IList<Node> cycle, IEnumerable<Node> except) {
    return cycle.Edges()
      .SelectMany(p => Instance.Nodes
        .Except(cycle)
        .Except(except)
        .Select(n => (p.b, n, gain: Instance.Gain.Insert(p, n))))
      .MinBy(x => x.gain);
  }

  public static void
    AppendFit(IList<Node> cycle, (Node previous, Node best, int gain) edge) {
    var (previous, best, _) = edge;
    cycle.Insert(cycle.IndexOf(previous), best);
  }


  public (Node previous, Node best, int gain)
    FindBestFitByWeightedRegretToInsertGain(IList<Node> cycle, IEnumerable<Node> except, int k, float weight) {
    return Instance
      .Nodes
      .Except(cycle)
      .Except(except)
      .Select(candidate =>
        cycle.Edges()
          .Select(edge => (edge.b, candidate, gain: Instance.Gain.Insert(edge, candidate)))
          .OrderBy(n => n.gain)
          .ToList()
      )
      .OrderBy(match => CalculateRegret(match.Select(x => x.gain), k) + weight * match.MinBy(x => x.gain).gain)
      .First()
      .MinBy(match => match.gain);
  }


  public (Node previous, Node best, int gain)
    FindBestFitByRegretInsertGain(IList<Node> cycle, IEnumerable<Node> except, int regret) {
    return Instance
      .Nodes
      .Except(cycle)
      .Except(except)
      .Select(candidate =>
        cycle.Edges()
          .Select(edge => (edge.b, candidate, gain: Instance.Gain.Insert(edge, candidate)))
          .OrderBy(n => n.gain)
          .ToList()
      )
      .OrderBy(match => CalculateRegret(match.Select(x => x.gain), regret))
      .First()
      .MinBy(match => match.gain);
  }

  public void PerformRandomMove(IEnumerable<IList<Node>> cycles) {
    var enumerated = cycles.ToList();

    var action = Random.Shared.Next(0, enumerated.Count > 1 ? 4 : 3);
    var first = Random.Shared.Choose(enumerated);

    switch (action) {
      case 1: {
        var a = Random.Shared.Choose(first);
        var b = Random.Shared.Choose(first.Except(a));
        ExchangeVertex(first, a, b);
        break;
      }
      case 2: {
        var second = Random.Shared.Choose(enumerated.Except(first));
        var a = Random.Shared.Choose(first);
        var b = Random.Shared.Choose(second);
        ExchangeVertex(first, second, a, b);
        break;
      }
      case 3: {
        var a = Random.Shared.Choose(first);
        var b = Random.Shared.Choose(first.Except(a));
        ExchangeEdge(first, a, b);
        break;
      }
    }
  }

  private static int CalculateRegret(IEnumerable<int> gains, int k) {
    var enumerable = gains as int[] ?? gains.ToArray();

    return enumerable.Skip(1).Take(k - 1).Aggregate(0, (acc, a) => acc + (enumerable.First() - a));
  }
}
