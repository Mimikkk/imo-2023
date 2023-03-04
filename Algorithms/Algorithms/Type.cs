using Algorithms.DataStructures;
using Ardalis.SmartEnum;

namespace Algorithms.Algorithms;

public class Algorithm : SmartEnum<Algorithm> {
  public static readonly Algorithm GreedyNearestNeighbour = new("greedy-nearest-neighbour");
  public static readonly Algorithm DoubleGreedyNearestNeighbour = new("double-greedy-nearest-neighbour");
  public static readonly Algorithm GreedyCycleExpansion = new("greedy-nearest-neighbour");
  public static readonly Algorithm GreedyCycleExpansionWith2Regret = new("greedy-nearest-neighbour");

  public Algorithm(string name)
    : base(name, _nextValue++) { }

  private static int _nextValue;
}
