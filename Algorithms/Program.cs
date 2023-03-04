using imo_2023.Algorithms;
using imo_2023.DataStructures;

const string HelpMessage = """
Usage: Algorithms <instance> <algorithm> ?--save ?--view
- Instance: kroA100, kroB100
- Algorithm: nearest-neighbour, cycle-expansion
- --save: Save the chart to the resources/graphs directory
- --view: View the chart in a window (Windows only)
""";
var ValidAlgorithms = new[] { "greedy-nearest-neighbour", "greedy-cycle-expansion" };
var ValidInstances = new[] { "kroA100", "kroB100" };

if (args.ElementAtOrDefault(0) == "--help") {
  WriteLine(HelpMessage);
  return 2;
}

var errors = new List<string>();
if (args.Length > 2) errors.Add("Too many arguments!");
else if (args.Length != 2) errors.Add("Too few arguments!");

if (!ValidInstances.Contains(args.ElementAtOrDefault(0))) errors.Add("Invalid instance!");
if (!ValidAlgorithms.Contains(args.ElementAtOrDefault(1))) errors.Add("Invalid algorithm!");
if (errors.Count > 0) {
  WriteLine(HelpMessage);
  errors.ForEach(e => WriteLine($"  - Error: {e}"));
  return 1;
}
var instance = Instance.Read(args[0]);
var algorithm = args[1];

var chart = CreateChart();
switch (algorithm) {
  case "greedy-nearest-neighbour": {
    var (first, second) = instance.SearchWithDoubleGreedyNearestNeighbour();

    chart.Add.Cycle(first, instance).Cycle(second, instance);
    chart.Title.Label.Text = "KroA100 - Greedy Nearest Neighbour";
    break;
  }
  case "greedy-cycle-expansion": {
    var path = instance.SearchWithGreedyCycleExpansion();

    chart.Add.Cycle(path, instance);
    chart.Title.Label.Text = "KroA100 - Greedy Cycle Expansion";
    chart.Save("Super");
    break;
  }
}
return 0;
