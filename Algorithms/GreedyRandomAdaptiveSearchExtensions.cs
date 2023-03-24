using System.Collections;
using System.Diagnostics;
using Algorithms.Structures;
using Ardalis.SmartEnum;
using Domain.Extensions;
using Domain.Structures;

namespace Algorithms;

internal static class GreedyRandomAdaptiveSearchExtensions {
  public static IEnumerable<IEnumerable<Node>>
    Search(this Instance instance, SearchConfiguration configuration) {
    var population = configuration.Population.ToArray();
    var timeLimit = configuration.TimeLimit;
    var initializer = configuration.Initializer;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));
    if (population.Flatten().Count() != instance.Dimension) initializer.Search(instance, configuration);

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      _ => SearchMultiple(instance, population, timeLimit),
    };
  }

  private static IEnumerable<IEnumerable<Node>>
    SearchMultiple(Instance instance, IEnumerable<ObservableList<Node>> population, float timeLimit) {
    var enumerable = population.ToArray();

    var bestSolutions = enumerable.Select(solution => solution.ToList()).ToList();
    var bestDistance = bestSolutions.Sum(solution => instance[solution]);

    var timer = Stopwatch.StartNew();
    while (timer.ElapsedMilliseconds < 1000 * timeLimit) {
      var solutions = bestSolutions.Select(solution => solution.ToList()).ToList();
      PerformRandomMove(solutions);

      var distance = solutions.Sum(solution => instance[solution]);
      if (distance >= bestDistance) continue;
      bestDistance = distance;
      bestSolutions = solutions;
      enumerable.Zip(solutions).ForEach(p => {
        p.First.Fill(p.Second);
        p.First.Notify();
      });
    }

    return bestSolutions;
  }

  private static void PerformRandomMove(List<List<Node>> cycles) {
    var moves = Move.List;
    if (cycles.Count == 1) moves = moves.Except(Move.TradeExternalTwoVertices).ToList();
    Random.Shared.Choose(moves).Invoke(cycles);
  }

  private class Move : SmartEnum<Move> {
    public static readonly Move TradeInternalTwoVertices = new(cycles => {
      var first = Random.Shared.Choose(cycles);
      var a = Random.Shared.Choose(first);
      var b = Random.Shared.Choose(first.Except(a));
      Moves.ExchangeVertex(first, a, b);
    });

    public static readonly Move TradeExternalTwoVertices = new(cycles => {
      var first = Random.Shared.Choose(cycles);
      var second = Random.Shared.Choose(cycles.Except(first));
      var a = Random.Shared.Choose(first);
      var b = Random.Shared.Choose(second);
      Moves.ExchangeVertex(first, second, a, b);
    });

    public static readonly Move TradeInternalTwoEdges = new(cycles => {
      var first = Random.Shared.Choose(cycles);
      var a = Random.Shared.Choose(first);
      var b = Random.Shared.Choose(first.Except(a));
      Moves.ExchangeEdge(first, a, b);
    });

    private Move(Action<List<List<Node>>> invoke) : base(_value++.ToString(), _value) => Invoke = invoke;
    private static int _value;
    public readonly Action<List<List<Node>>> Invoke;
  }
}
