using imo_2023.DataStructures;
using ScottPlot;

namespace imo_2023.Methods;

public static class ChartMethods {
  public static Plot CreateChart() => new();

  public static AddPlottable Cycle(this AddPlottable add, IEnumerable<Node> cycle, Instance instance) {
    var nodes = cycle as Node[] ?? cycle.ToArray();

    var scatter = add.Scatter(
      xs: nodes.Select(node => (double)node.X).Append(nodes.First().X).ToArray(),
      ys: nodes.Select(node => (double)node.Y).Append(nodes.First().Y).ToArray()
    );
    scatter.Label = $"Cycle with length: {instance.DistanceOf(nodes.Append(nodes.First()))}";

    return add;
  }

  public static void Save(this Plot chart, string filename) {
    if (!Directory.Exists($"{ProjectDirectory}/resources/graphs")) Directory.CreateDirectory($"{ProjectDirectory}/resources/graphs");

    chart.SavePng($"{ProjectDirectory}/resources/graphs/{filename}.png", 1200, 800);
  }
}
