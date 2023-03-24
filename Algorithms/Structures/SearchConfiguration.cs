using Domain.Structures;

namespace Algorithms.Structures;

public record SearchConfiguration {
  public float Weight;
  public int? Start;
  public float TimeLimit;
  public int Regret;
  public Algorithm? Initializer = null;
  public string Variant;
  public readonly List<int> Gains = new();
  public List<ObservableList<Node>> Population = null!;
}
