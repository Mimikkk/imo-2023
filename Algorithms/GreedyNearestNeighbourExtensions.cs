using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyNearestNeighbourExtensions {
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
      _ => instance.SearchMultiple(population)
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchSingle(this Instance instance, IList<Node>? path, int? start) {
    path ??= new List<Node>();
    path.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);

    while (path.Count < instance.Dimension) instance.Move.AppendClosestToHeadOrTail(path, path);

    return Yield(path);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(this Instance instance, IList<Node>? first, IList<Node>? second, int? start) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    second ??= new List<Node>();
    second.Add(instance.Move.FurthestTo(first.First()));

    while (first.Count < instance.Dimension / 2) {
      instance.Move.AppendClosestToHeadOrTail(first, first.Concat(second));
      instance.Move.AppendClosestToHeadOrTail(second, first.Concat(second));
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<IList<Node>> paths) {
    paths = paths.ToArray();

    var points = instance.Move.FindFurthest(paths.Count());
    foreach (var (path, point) in paths.Zip(points)) path.Add(point);

    while (true) {
      foreach (var path in paths) {
        instance.Move.AppendClosestToHeadOrTail(path, paths.Flatten());
        if (paths.Flatten().Count() == instance.Dimension) return paths;
      }
    }
  }
}
