using Ardalis.SmartEnum;
using Domain.Structures;

namespace Algorithms.Structures;

public sealed class Algorithm : SmartEnum<Algorithm> {
  public static readonly Algorithm NearestNeighbour = new(
    DisplayType.Path,
    GreedyNearestNeighbourExtensions.Search,
    false,
    false
  );

  public static readonly Algorithm CycleExpansion = new(
    DisplayType.Cycle,
    GreedyCycleExpansionExtensions.Search,
    false,
    false
  );

  public static readonly Algorithm CycleExpansionWithKRegret = new(
    DisplayType.Cycle,
    GreedyRegretCycleExpansionExtensions.Search,
    true,
    false
  );

  public static readonly Algorithm CycleExpansionWithKRegretAndWeight = new(
    DisplayType.Cycle,
    GreedyWeightedRegretCycleExpansionExtensions.Search,
    true,
    true
  );

  public static readonly Algorithm Random = new(
    DisplayType.Cycle,
    RandomSearchExtensions.Search,
    false,
    false
  );

  public static readonly Algorithm RandomAdaptive = new(
    DisplayType.Cycle,
    GreedyRandomAdaptiveSearchExtensions.Search,
    false,
    false
  );

  public Algorithm(
    DisplayType displayAs,
    Func<Instance, SearchConfiguration, IEnumerable<IEnumerable<Node>>> search,
    bool usesRegret,
    bool usesWeight
  )
    : base(_nextValue.ToString(), ++_nextValue) {
    DisplayAs = displayAs;
    Search = search;
    UsesRegret = usesRegret;
    UsesWeight = usesWeight;
  }

  public enum DisplayType {
    Cycle,
    Path
  }

  public readonly bool UsesRegret;
  public readonly bool UsesWeight;
  public readonly DisplayType DisplayAs;
  private static int _nextValue;

  public readonly Func<Instance, SearchConfiguration, IEnumerable<IEnumerable<Node>>> Search;

  public static implicit operator string(Algorithm algorithm) => algorithm.Name;
}
