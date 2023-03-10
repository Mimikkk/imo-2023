using Domain.Structures;

namespace Algorithms.Structures;

public record SearchConfiguration {
  public float Weight;
  public int? Start;
  public int Regret;
  public IEnumerable<IList<Node>> Population = null!;
}
