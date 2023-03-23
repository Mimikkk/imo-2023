using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyCycleExpansionExtensions {
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
    SearchSingle(Instance instance, IList<Node>? cycle, int? start) {
    cycle ??= new List<Node>();
    cycle.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    cycle.Add(instance.Move.ClosestTo(cycle.First()));

    while (cycle.Count < instance.Dimension)
      Moves.AppendFit(cycle, instance.Move.FindBestFitByLowestGain(cycle, cycle));

    return Yield(cycle);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchDouble(Instance instance, IList<Node>? first, IList<Node>? second, int? start) {
    first ??= new List<Node>();
    first.Add(start is null ? Node.Choose(instance.Nodes) : instance.Nodes[start.Value]);
    first.Add(instance.Move.ClosestTo(first.First()));

    second ??= new List<Node>();
    second.Add(instance.Move.FurthestTo(first.First()));
    second.Add(instance.Move.ClosestTo(second.First()));

    while (first.Count < instance.Dimension / 2) {
      Moves.AppendFit(first, instance.Move.FindBestFitByLowestGain(first, second));
      Moves.AppendFit(second, instance.Move.FindBestFitByLowestGain(second, first));
    }

    return Yield(first, second);
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(this Instance instance, IEnumerable<IList<Node>> paths) {
    paths = paths.ToArray();

    var points = instance.Move.FindFurthest(paths.Count());
    foreach (var (path, point) in paths.Zip(points)) path.Add(point);
    foreach (var path in paths) instance.Move.ClosestTo(path.First(), paths.Flatten());

    var count = paths.Flatten().Count();
    while (true) {
      foreach (var path in paths) {
        Moves.AppendFit(path, instance.Move.FindBestFitByLowestGain(path, paths.Flatten().Except(path)));
        if (++count == instance.Dimension) return paths;
      }
    }
  }
}
