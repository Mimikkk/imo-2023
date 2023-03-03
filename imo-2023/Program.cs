using imo_2023.Algorithms;
using imo_2023.DataStructures;
using ScottPlot.WinForms;

const string HelpMessage = """
Usage: imo-2023 <instance> <algorithm> ?--save ?--view
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
    break;
  }
}

var parent = new Form();
var form = new FormsPlot();
form.View(chart);
form.Parent = parent;
parent.HandleCreated += (_, _) => {
  form.Width = parent.ClientSize.Width;
  form.Height = parent.ClientSize.Height;
};
parent.Resize += (sender, _) => {
  var control = (Control)sender!;

  form.Width = parent.ClientSize.Width;
  form.Height = parent.ClientSize.Height;
  chart.Add.Scatter(new double[] { control.Size.Width }, new double[] { control.Size.Height });
};

parent.Show();
while (parent.Created) Application.DoEvents();

return 0;
