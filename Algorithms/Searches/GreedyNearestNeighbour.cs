using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

internal sealed class GreedyNearestNeighbour : ISearch {
  public static IEnumerable<IEnumerable<Node>>
    Search(Instance instance, ISearch.Configuration configuration) {
    var population = configuration.Population.ToArray();
    var start = configuration.Start;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0   => Enumerable.Empty<IEnumerable<Node>>(),
      1   => Single(instance, population.First(), start),
      2   => Double(instance, population.First(), population.Last(), start),
      _   => Multiple(instance, population)
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    Single(Instance instance, ObservableList<Node> path, int? start) {
    path.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    path.Notify();

    while (path.Count < instance.Dimension) {
      instance.Move.AppendClosestToHeadOrTail(path, path);
      path.Notify();
    }

    return Yield(path);
  }

  private static IEnumerable<IEnumerable<Node>>
    Double(Instance instance, ObservableList<Node> first, ObservableList<Node> second, int? start) {
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
    Multiple(Instance instance, IEnumerable<ObservableList<Node>> paths) {
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
