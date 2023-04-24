using Algorithms.Searches;
using Algorithms.Structures;
using Domain;
using Domain.Structures;
using Domain.Structures.Instances;

var search = Algorithm.SteepestLocal.Search;
var instance = Instance.Predefined.KroA200;
Globals.Random = new(42);

var configuration = new ISearch.Configuration {
  Initializer = Algorithm.Random,
  Population = Times(2, ObservableList<Node>.Create).ToList(),
  Variant = "mixed"
};

var start = DateTime.Now;
search(instance, configuration);
var end = DateTime.Now;
var elapsed = end - start;

Console.WriteLine($"Elapsed time: in second {elapsed.TotalSeconds:F2}");
