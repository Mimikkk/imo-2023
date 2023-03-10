using System.Diagnostics;
using Algorithms.DataStructures;
using Algorithms.Extensions;
using static Algorithms.Algorithms.Algorithm;

namespace Algorithms.Algorithms;

public static class GreedyCycleExpansionExtensions {
  private static (Node previous, Node best) FindBestFitByLowestGain(this Instance instance, IList<Node> cycle,
    IEnumerable<Node> except) =>
    cycle.Edges()
      .SelectMany(p => instance.Nodes.Except(cycle)
        .Except(except)
        .Select(n => (p.b, n, cost: instance.InsertCost(p, n))))
      .MinBy(x => x.cost)
      .DropLast();

  public static IEnumerable<Node>
    SearchWithGreedyCycleExpansion(this Instance instance, IList<Node>? cycle = null, int? start = null) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension) {
      var (previous, best) = instance.FindBestFitByLowestGain(cycle, cycle);

      cycle.Insert(cycle.IndexOf(previous), best);
    }

    return cycle;
  }

  public static (IEnumerable<Node>, IEnumerable<Node>)
    SearchWithGreedyCycleExpansion(this Instance instance, IList<Node>? first = null, IList<Node>? second = null,
      int? start = null) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Add(instance.ClosestTo(first.First()));

    second ??= new List<Node>();
    second.Add(instance.FurthestTo(first.First()));
    second.Add(instance.ClosestTo(second.First()));

    while (first.Count < instance.Dimension / 2) {
      var (previous, best) = instance.FindBestFitByLowestGain(first, second);
      first.Insert(first.IndexOf(previous), best);

      (previous, best) = instance.FindBestFitByLowestGain(second, first);
      second.Insert(second.IndexOf(previous), best);
    }

    return (first, second);
  }


  public static IEnumerable<IEnumerable<Node>> Search(this Instance instance, SearchConfiguration configuration) {
    var paths = configuration.population.ToArray();

    var points = instance.ChooseFurthest(paths.Length);
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
}
