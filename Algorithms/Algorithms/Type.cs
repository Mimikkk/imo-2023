using Algorithms.DataStructures;
using Ardalis.SmartEnum;

namespace Algorithms.Algorithms;

public class Algorithm : SmartEnum<Algorithm> {
  public static readonly Algorithm GreedyNearestNeighbour = new();
  public static readonly Algorithm DoubleGreedyNearestNeighbour = new();
  public static readonly Algorithm GreedyCycleExpansion = new();
  public static readonly Algorithm GreedyCycleExpansionWith2Regret = new();

  public Algorithm()
    : base(_nextValue.ToString(), ++_nextValue) { }

  private static int _nextValue;

  public static implicit operator string(Algorithm algorithm) => algorithm.Name;
}
