using Domain.Structures;

namespace Algorithms.Structures;

public sealed record SearchConfiguration(IEnumerable<IList<Node>> population, int? regret = null, int? start = null);
