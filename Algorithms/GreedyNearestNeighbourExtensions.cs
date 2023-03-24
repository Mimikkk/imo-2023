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
    SearchSingle(this Instance instance, ObservableList<Node> path, int? start) {
    path.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    path.Notify();

    while (path.Count < instance.Dimension) {
      instance.Move.AppendClosestToHeadOrTail(path, path);
      path.Notify();
    }

    return Yield(path);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(this Instance instance, ObservableList<Node> first, ObservableList<Node> second, int? start) {
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Notify();
    second.Add(instance.Move.FurthestTo(first.First()));
    second.Notify();

    while (true) {
      if (first.Count + second.Count == instance.Dimension) break;
      instance.Move.AppendClosestToHeadOrTail(first, first.Concat(second));
      first.Notify();
      if (first.Count + second.Count == instance.Dimension) break;
      instance.Move.AppendClosestToHeadOrTail(second, first.Concat(second));
      second.Notify();
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<ObservableList<Node>> paths) {
    paths = paths.ToArray();

    var points = instance.Move.FindFurthest(paths.Count());
    foreach (var (path, point) in paths.Zip(points)) path.Add(point);

    while (true) {
      foreach (var path in paths) {
        instance.Move.AppendClosestToHeadOrTail(path, paths.Flatten());
        path.Notify();
        if (paths.Flatten().Count() == instance.Dimension) return paths;
      }
    }
  }
}
