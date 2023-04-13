using Algorithms.Structures;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

public interface ISearch {
  internal static abstract IEnumerable<IEnumerable<Node>> Search(Instance instance, Configuration configuration);
 
  public record Configuration {
    public float Weight;
    public int? Start;
    public float TimeLimit;
    public int Regret;
    public Algorithm? Initializer = null;
    public string Variant;
    public readonly List<int> Gains = new();
    public List<ObservableList<Node>> Population = null!;
  }
}
