using Ardalis.SmartEnum;
using Domain.Structures;

namespace Algorithms.Structures;

public sealed class Algorithm : SmartEnum<Algorithm> {
  public static readonly Algorithm NGreedyNearestNeighbour = new(StrategyType.PathBased, GreedyNearestNeighbourExtensions.Search);
  public static readonly Algorithm NGreedyCycleExpansion = new(StrategyType.CycleBased, GreedyCycleExpansionExtensions.Search);
  public static readonly Algorithm NGreedyCycleExpansionWithKRegret = new(StrategyType.CycleBased, GreedyRegretCycleExpansionExtensions.Search);
  public static readonly Algorithm NGreedyCycleExpansionWithKRegretAndWeight =
    new(StrategyType.CycleBased, GreedyWeightedRegretCycleExpansionExtensions.Search);

  public Algorithm(StrategyType strategyType, Func<Instance, SearchConfiguration, IEnumerable<IEnumerable<Node>>> search)
    : base(_nextValue.ToString(), ++_nextValue) {
    Type = strategyType;
    Search = search;
  }

  public enum StrategyType { CycleBased, PathBased }
  public readonly StrategyType Type;
  private static int _nextValue;

  public readonly Func<Instance, SearchConfiguration, IEnumerable<IEnumerable<Node>>> Search;

  public static implicit operator string(Algorithm algorithm) => algorithm.Name;
}
