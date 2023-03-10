using System.Diagnostics;
using Algorithms.DataStructures;
using Algorithms.Extensions;

namespace Algorithms.Algorithms;

public static class GreedyCycleExpansionExtensions {
  private static (Node previous, Node best)
    FindBestFitByLowestGain(this Instance instance, IList<Node> cycle, IEnumerable<Node> except) =>
    cycle.Edges()
      .SelectMany(p => instance.Nodes
        .Except(cycle)
        .Except(except)
        .Select(n => (p.b, n, cost: instance.InsertCost(p, n))))
      .MinBy(x => x.cost)
      .DropLast();

  private static void
    FindAndAppendBestFit(this Instance instance, IList<Node> cycle, IEnumerable<Node> except) {
    var (previous, best) = instance.FindBestFitByLowestGain(cycle, except);
    cycle.Insert(cycle.IndexOf(previous), best);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(Instance instance, IList<Node>? cycle, int? start) {
    Debug.WriteLine("Sin");
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension) instance.FindAndAppendBestFit(cycle, cycle);

    return Yield(cycle);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(Instance instance, IList<Node>? first, IList<Node>? second, int? start) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Add(instance.ClosestTo(first.First()));

    second ??= new List<Node>();
    second.Add(instance.FurthestTo(first.First()));
    second.Add(instance.ClosestTo(second.First()));

    while (first.Count < instance.Dimension / 2) {
      instance.FindAndAppendBestFit(first, second);
      instance.FindAndAppendBestFit(second, first);
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<IList<Node>> paths) {
    paths = paths.ToArray();

    var points = instance.ChooseFurthest(paths.Count());
    foreach (var (path, point) in paths.Zip(points)) path.Add(point);
    foreach (var path in paths) instance.ClosestTo(path.First(), paths.Flatten());

    var count = paths.Flatten().Count();
    while (true) {
      foreach (var path in paths) {
        var (previous, best) = instance.FindBestFitByLowestGain(path, paths.Flatten().Except(path));
        path.Insert(path.IndexOf(previous), best);
        if (++count == instance.Dimension) return paths;
      }
    }
  }

  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
    var population = configuration.population.ToArray();
    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));
    Debug.WriteLine("HHH");
    Debug.WriteLine(population.Length);

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0   => Enumerable.Empty<IEnumerable<Node>>(),
      1   => SearchSingle(instance, population.First(), configuration.start),
      2   => SearchDouble(instance, population.First(), population.Last(), configuration.start),
      _   => instance.SearchMultiple(configuration.population)
    };
  }
}
