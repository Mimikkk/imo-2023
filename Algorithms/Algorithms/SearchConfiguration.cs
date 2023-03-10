using Algorithms.DataStructures;

namespace Algorithms.Algorithms;

public record SearchConfiguration(IEnumerable<IList<Node>> population, int? regret = null, int? start = null);
