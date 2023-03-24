using Domain.Structures;

namespace Algorithms.Structures;

public record SearchConfiguration {
  public float Weight;
  public int? Start;
  public float TimeLimit;
  public int Regret;
  public Algorithm? Initializer = null;
  public List<ObservableList<Node>> Population = null!;
}
