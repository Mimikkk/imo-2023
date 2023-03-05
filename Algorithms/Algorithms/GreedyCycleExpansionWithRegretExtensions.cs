using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyRegretCycleExpansionExtensions {
  private static IEnumerable<(Node, Node, int)> FindFitsByGain(this Instance instance, IList<Node> cycle, IEnumerable<Node> except) => Enumerable
    .Range(1, cycle.Count - 1)
    .Select(x => (x, x - 1))
    .Concat(Yield((0, cycle.Count - 1)))
    .Select(p => (cycle[p.Item1], cycle[p.Item2]))
    .SelectMany(p => instance.Nodes.Except(cycle)
      .Except(except)
      .Select(n => (p.Item1, n, instance[n, p.Item1] + instance[n, p.Item2] - instance[p.Item1, p.Item2])))
    .OrderBy(x => x.Item3);

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansionWithKRegret(this Instance instance, int regret, IList<Node>? cycle = null, int? start = null) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension) {
      var fits = instance.FindFitsByGain(cycle, cycle).Take(1 + regret);


      var (previous, best, _) = instance.FindFitsByGain(cycle, cycle).First();

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
