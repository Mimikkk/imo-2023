using Ardalis.SmartEnum;

namespace Algorithms.Algorithms;

public class Algorithm : SmartEnum<Algorithm> {
  public static readonly Algorithm GreedyNearestNeighbour = new(StrategyType.PathBased);
  public static readonly Algorithm DoubleGreedyNearestNeighbour = new(StrategyType.PathBased);
  public static readonly Algorithm GreedyCycleExpansion = new(StrategyType.CycleBased);
  public static readonly Algorithm DoubleGreedyCycleExpansion = new(StrategyType.CycleBased);
  public static readonly Algorithm GreedyCycleExpansionWith2Regret = new(StrategyType.CycleBased);
  public static readonly Algorithm DoubleGreedyCycleExpansionWith2Regret = new(StrategyType.CycleBased);

  public Algorithm(StrategyType strategyType)
    : base(_nextValue.ToString(), ++_nextValue) {
    Type = strategyType;
  }

  public enum StrategyType { CycleBased, PathBased }
  public readonly StrategyType Type;
  private static int _nextValue;

  public static implicit operator string(Algorithm algorithm) => algorithm.Name;
}
