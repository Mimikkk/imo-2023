using Ardalis.SmartEnum;
using Domain.Structures;

namespace Algorithms.Structures;

using SearchFn = Func<Instance, SearchConfiguration, IEnumerable<IEnumerable<Node>>>;

public sealed class Algorithm : SmartEnum<Algorithm> {
  public static readonly Algorithm NearestNeighbour = new(
    DisplayType.Path,
    GreedyNearestNeighbour.Search
  );

  public static readonly Algorithm CycleExpansion = new(
    DisplayType.Cycle,
    GreedyCycleExpansion.Search
  );

  public static readonly Algorithm CycleExpansionWithKRegret = new(
    DisplayType.Cycle,
    GreedyRegretCycleExpansion.Search,
    usesRegret: true
  );

  public static readonly Algorithm CycleExpansionWithKRegretAndWeight = new(
    DisplayType.Cycle,
    GreedyWeightedRegretCycleExpansion.Search,
    usesRegret: true,
    usesWeight: true
  );

  public static readonly Algorithm Random = new(
    DisplayType.Cycle,
    RandomSearch.Search
  );

  public static readonly Algorithm RandomAdaptive = new(
    DisplayType.Cycle,
    GreedyRandomAdaptiveSearch.Search,
    usesTimeLimit: true,
    usesInitializer: true
  );

  public static readonly Algorithm GreedyLocal = new(
    DisplayType.Cycle,
    GreedyLocalSearch.Search,
    usesInitializer: true,
    usesVariant: true,
    usesRegret: true,
    usesWeight: true
  );

  public static readonly Algorithm SteepestLocal = new(
    DisplayType.Cycle,
    SteepestLocalSearch.Search,
    usesInitializer: true,
    usesVariant: true,
    usesRegret: true,
    usesWeight: true
  );

  private Algorithm(
    DisplayType displayAs,
    SearchFn search,
    bool usesRegret = false,
    bool usesWeight = false,
    bool usesTimeLimit = false,
    bool usesVariant = false,
    bool usesInitializer = false)
    : base(_nextValue.ToString(), ++_nextValue) {
    DisplayAs = displayAs;
    Search = search;
    UsesRegret = usesRegret;
    UsesWeight = usesWeight;
    UsesTimeLimit = usesTimeLimit;
    UsesInitializer = usesInitializer;
    UsesVariant = usesVariant;
  }

  public enum DisplayType {
    Cycle,
    Path
  }

  public readonly bool UsesRegret;
  public readonly bool UsesWeight;
  public readonly bool UsesTimeLimit;
  public readonly bool UsesInitializer;
  public readonly bool UsesVariant;
  public readonly DisplayType DisplayAs;
  private static int _nextValue;

  public readonly SearchFn Search;

  public static implicit operator string(Algorithm algorithm) => algorithm.Name;
}
