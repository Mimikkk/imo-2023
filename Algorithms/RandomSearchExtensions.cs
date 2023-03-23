using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class RandomSearchExtensions {
  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
    var population = configuration.Population.ToArray();
    var start = configuration.Start;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      1 => SearchSingle(instance, population.First(), start),
      2 => SearchDouble(instance, population.First(), population.Last(), start),
      _ => SearchMultiple(instance, population)
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(Instance instance, IList<Node>? cycle, int? start) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);

    while (cycle.Count < instance.Dimension) cycle.Add(Node.Choose(instance.Nodes.Except(cycle)));

    return Yield(cycle);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(Instance instance, IList<Node>? first, IList<Node>? second, int? start) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);

    second ??= new List<Node>();
    second.Add(instance.Move.FurthestTo(first.First()));

    while (true) {
      first.Add(Node.Choose(instance.Nodes.Except(first).Except(second)));
      if (first.Count + second.Count == instance.Dimension) break;
      second.Add(Node.Choose(instance.Nodes.Except(first).Except(second)));
      if (first.Count + second.Count == instance.Dimension) break;
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<IList<Node>> cycles) {
    cycles = cycles.ToArray();

    foreach (var (path, point) in cycles.Zip(instance.Move.FindFurthest(cycles.Count()))) path.Add(point);
    var count = cycles.Flatten().Count();

    while (true) {
      foreach (var cycle in cycles) {
        cycle.Add(Node.Choose(instance.Nodes.Except(cycles.Flatten())));
        if (++count == instance.Dimension) return cycles;
      }
    }
  }
}
