using System.Diagnostics;
using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyNearestNeighbourExtensions {
  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
    var population = configuration.population.ToArray();
    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0   => Enumerable.Empty<IEnumerable<Node>>(),
      1   => instance.SearchSingle(population.First(), configuration.start),
      2   => instance.SearchDouble(population.First(), population.Last(), configuration.start),
      _   => instance.SearchMultiple(population, configuration.start)
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(this Instance instance, IList<Node>? path, int? start) {
    path ??= new List<Node>();
    path.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);

    while (path.Count < instance.Dimension) instance.AppendClosestHeadOrTail(path, path);

    return Yield(path);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(this Instance instance, IList<Node>? first, IList<Node>? second, int? start) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    second ??= new List<Node>();
    second.Add(instance.FurthestTo(first.First()));

    while (first.Count < instance.Dimension / 2) {
      instance.AppendClosestHeadOrTail(first, first.Concat(second));
      instance.AppendClosestHeadOrTail(second, first.Concat(second));
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<IList<Node>> paths, int? start) {
    paths = paths.ToArray();

    if (start is not null) {
      var hull = instance.Nodes.Hull();
      paths.First().Add(Node.Choose(hull));
    }
    var points = instance.ChooseFurthest(paths.Count());
    foreach (var (path, point) in paths.Zip(points)) path.Add(point);

    while (true) {
      foreach (var path in paths) {
        instance.AppendClosestHeadOrTail(path, paths.Flatten());
        if (paths.Flatten().Count() == instance.Dimension) return paths;
      }
    }
  }

  private static void
    AppendClosestHeadOrTail(this Instance instance, IList<Node> path, IEnumerable<Node> except) {
    var excepted = except.ToArray();
    var closestToTail = instance.ClosestTo(path.First(), excepted);
    var closestToHead = instance.ClosestTo(path.Last(), excepted);

    if (instance[closestToTail, path.First()] > instance[path.Last(), closestToHead]) path.Add(closestToHead);
    else path.Insert(0, closestToTail);
  }
}
