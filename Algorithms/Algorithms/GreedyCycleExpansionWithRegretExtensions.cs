using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public static class GreedyRegretCycleExpansionExtensions {
  private static IEnumerable<(Node, Node, int)> FindFitByLowestGain(this Instance instance, IList<Node> cycle, int distance, IEnumerable<Node> except)
    => Enumerable
      .Range(1, cycle.Count - 1)
      .Select(x => (x, x - 1))
      .Concat(Yield((0, cycle.Count - 1)))
      .Select(p => (cycle[p.Item1], cycle[p.Item2], distance - instance[cycle[p.Item1], cycle[p.Item2]]))
      .SelectMany(p => instance.Nodes.Except(cycle)
        .Except(except)
        .Select(n => (p.Item1, n, p.Item3 + instance[n, p.Item1] + instance[n, p.Item2])))
      .OrderBy(x => x.Item3);

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansionWithKRegret(this Instance instance, int regret, IList<Node>? cycle = null, int? start = null) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    var distance = instance.DistanceOf(cycle);
    while (cycle.Count < instance.Dimension) {
      var fits = instance.FindFitByLowestGain(cycle, distance, cycle).Take(1 + regret);
      
      
      (var previous, var best, distance) = instance.FindFitByLowestGain(cycle, distance, cycle).First();

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