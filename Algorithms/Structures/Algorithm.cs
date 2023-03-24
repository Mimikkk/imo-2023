using Ardalis.SmartEnum;
using Domain.Structures;

namespace Algorithms.Structures;

using SearchFn = Func<Instance, SearchConfiguration, IEnumerable<IEnumerable<Node>>>;

public sealed class Algorithm : SmartEnum<Algorithm> {
  public static readonly Algorithm NearestNeighbour = new(
    DisplayType.Path,
    GreedyNearestNeighbourExtensions.Search,
    false,
    false,
    false,
    false
  );

  public static readonly Algorithm CycleExpansion = new(
    DisplayType.Cycle,
    GreedyCycleExpansionExtensions.Search,
    false,
    false,
    false,
    false
  );

  public static readonly Algorithm CycleExpansionWithKRegret = new(
    DisplayType.Cycle,
    GreedyRegretCycleExpansionExtensions.Search,
    true,
    false,
    false,
    false
  );

  public static readonly Algorithm CycleExpansionWithKRegretAndWeight = new(
    DisplayType.Cycle,
    GreedyWeightedRegretCycleExpansionExtensions.Search,
    true,
    true,
    false,
    false
  );

  public static readonly Algorithm Random = new(
    DisplayType.Cycle,
    RandomSearchExtensions.Search,
    false,
    false,
    false,
    false
  );

  public static readonly Algorithm RandomAdaptive = new(
    DisplayType.Cycle,
    GreedyRandomAdaptiveSearchExtensions.Search,
    false,
    false,
    true,
    true
  );

  private Algorithm(
    DisplayType displayAs,
    SearchFn search,
    bool usesRegret,
    bool usesWeight, bool usesTimeLimit, bool usesInitializer)
    : base(_nextValue.ToString(), ++_nextValue) {
    DisplayAs = displayAs;
    Search = search;
    UsesRegret = usesRegret;
    UsesWeight = usesWeight;
    UsesTimeLimit = usesTimeLimit;
    UsesInitializer = usesInitializer;
  }

  public enum DisplayType {
    Cycle,
    Path
  }

  public readonly bool UsesRegret;
  public readonly bool UsesWeight;
  public readonly bool UsesTimeLimit;
  public readonly bool UsesInitializer;
  public readonly DisplayType DisplayAs;
  private static int _nextValue;

  public readonly SearchFn Search;

  public static implicit operator string(Algorithm algorithm) => algorithm.Name;
}
