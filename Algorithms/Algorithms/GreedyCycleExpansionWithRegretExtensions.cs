using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyRegretCycleExpansionExtensions {
  private static IEnumerable<(Node, Node, int)> FindFitsByGain(this Instance instance, IList<Node> cycle, IEnumerable<Node> except) {
    return Enumerable
      .Range(1, cycle.Count - 1)
      .Select(x => (x, x - 1))
      .Concat(Yield((0, cycle.Count - 1)))
      .Select(p => (cycle[p.Item1], cycle[p.Item2]))
      .SelectMany(p => instance.Nodes.Except(cycle)
        .Except(except)
        .Select(n => (p.Item1, n, instance[n, p.Item1] + instance[n, p.Item2] - instance[p.Item1, p.Item2])))
      .OrderBy(x => x.Item3);
  }
  private static (Node, Node, int) FindFitsByRegretGain(this Instance instance, IList<Node> cycle, IEnumerable<Node> except, int depth) {
    var edges = cycle.Edges();

    return Enumerable
      .Range(1, cycle.Count - 1)
      .Select(x => (x, y: x - 1))
      .Concat(Yield((x: 0, y: cycle.Count - 1)))
      .Select(p => (a: cycle[p.x], b: cycle[p.y]))
      .Select(p => {
        var pairs = instance.Nodes.Except(except)
          .Select(n => (p.a, n, distance: instance[n, p.a] + instance[n, p.b] - instance[p.a, p.b]))
          .OrderBy(n => n.distance)
          .ToList();

        var regret = pairs.Skip(1).Take(depth - 1).Sum(p => pairs.First().distance - p.distance);

        return (p.a, pairs.First().n, regret);

      })
      .MinBy(x => x.regret);

  }

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansionWithKRegret(this Instance instance, int regret, IList<Node>? cycle = null, int? start = null) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension) {
      var (previous, best, _) = FindFitsByRegretGain(instance, cycle, cycle, regret);

      cycle.Insert(cycle.IndexOf(previous), best);
    }

    return cycle;
  }
  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansionWith2Regret(this Instance instance, IList<Node>? cycle = null, int? start = null)
    => SearchWithGreedyCycleExpansionWithKRegret(instance, 2, cycle, start);
}
// Dla każdego wierzchołka oblicz koszt wstawienia w miejsca, każdej krawędzi.
// Z obliczonych kosztów wybierz krawędź o najmniejszym koszcie k kolejnych wstawień.
// a 1 2 3 4 5
// b 5 6 7 8 9
// żal = a = 1 - 5 = -4
// żal = b = 5 - 1 = 4
