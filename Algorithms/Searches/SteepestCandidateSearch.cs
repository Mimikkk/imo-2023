using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

internal sealed class SteepestCandidateSearch : ISearch {
  public static IEnumerable<IEnumerable<Node>>
    Search(Instance instance, ISearch.Configuration configuration) {
    var population = configuration.Population.ToArray();
    var initializer = configuration.Initializer;
    var gains = configuration.Gains;

    var hullSize = instance.Nodes.Hull().Count();
    if (population.Length > hullSize) throw new ArgumentOutOfRangeException(nameof(configuration));
    if (population.Flatten().Count() != instance.Dimension) initializer!.Search(instance, configuration);

    return population.Length switch {
      < 0 => throw new ArgumentOutOfRangeException(nameof(configuration)),
      0 => Enumerable.Empty<IEnumerable<Node>>(),
      _ => Multiple(instance, population, gains),
    };
  }

  private static IEnumerable<(Node a, Node b)> CreateCandidates(Instance instance, int size) =>
    instance.Nodes.SelectMany(a =>
        instance.Nodes.Except(a)
          .Select(b => ((a, b), distance: instance[a, b]))
          .OrderBy(edge => edge.distance)
          .Take(size))
      .Select(n => n.DropLast());


  private static IEnumerable<IEnumerable<Node>>
    Multiple(
      Instance instance,
      IEnumerable<ObservableList<Node>> population,
      ICollection<int> gains
    ) {
    var enumerable = population.ToArray();
    var cycles = enumerable.Select(solution => solution.ToList()).ToList();
    var candidates = CreateCandidates(instance, 10);

    while (true) {
      // TODO: Implement as a tuple not this fucking mess
      (List<Node> a, List<Node> b)? best_cycle = null;
      var best_gain = int.MinValue;
      (Node a, Node b)? best_edge = null;
      string? best_type = null;
      
      foreach (var candidate in candidates) {
        var first = cycles.Find(c => c.Contains(candidate.a))!;
        var second = cycles.Find(c => c.Contains(candidate.b))!;

        if (first == second) {

          var gain = instance.Gain.ExchangeEdge(first, candidate.a, candidate.b);
          if (gain > best_gain) {
            best_edge = candidate;
            best_gain = gain;
            best_cycle = (first, second);
            best_type = "edge";
          }
        }
        else {
          var gain = instance.Gain.ExchangeVertex(first, second, candidate.a, candidate.b);
          if (gain > best_gain) {
            best_edge = candidate;
            best_gain = gain;
            best_cycle = (first, second);
            best_type = "vertex";
          }
        }
      }

      if (best_gain > 0) {
        if (best_type == "edge") {
          Moves.ExchangeEdge(best_cycle.Value.a, best_edge.Value.a, best_edge.Value.b);
        }

        if (best_type == "vertex") {
          Moves.ExchangeVertex(best_cycle.Value.a, best_cycle.Value.b, best_edge.Value.a, best_edge.Value.b);
        }

        Notify(enumerable, cycles);
      }
      else break;
    }

    

    return cycles;
  }
  private static void Notify(IEnumerable<ObservableList<Node>> observables, IEnumerable<List<Node>> cycles) {
        observables.Zip(cycles)
          .ForEach(p => {
            p.First.Fill(p.Second);
            p.First.Notify();
          });
      }

}
