namespace Algorithms.DataStructures;

public record SearchConfiguration(IEnumerable<IList<Node>> population, int? regret = null, int? start = null);
