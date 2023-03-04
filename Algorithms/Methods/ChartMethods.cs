using Algorithms.DataStructures;
using ScottPlot;
using ScottPlot.Extensions;

namespace Algorithms.Methods;

public static class ChartMethods {
  public static AddPlottable Cycle(this AddPlottable add, IEnumerable<Node> cycle, Instance instance) {
    var nodes = cycle as Node[] ?? cycle.ToArray();

    var scatter = add.Scatter(
      xs: nodes.Select(node => (double)node.X).Append(nodes.First().X).ToArray(),
      ys: nodes.Select(node => (double)node.Y).Append(nodes.First().Y).ToArray()
    );
    scatter.Label = $"Długość cyklu: {instance.DistanceOf(nodes.Append(nodes.First()))}";

    return add;
  }

  public static AddPlottable Path(this AddPlottable add, IEnumerable<Node> cycle, Instance instance) {
    var nodes = cycle as Node[] ?? cycle.ToArray();

    var scatter = add.Scatter(
      xs: nodes.Select(node => (double)node.X).ToArray(),
      ys: nodes.Select(node => (double)node.Y).ToArray()
    );
    scatter.Label = $"Długość ścieżki: {instance.DistanceOf(nodes.Append(nodes.First()))}";

    return add;
  }

  public static AddPlottable Scatter(this AddPlottable add, IEnumerable<Node> nodes, Instance instance) {
    nodes = nodes.ToArray();

    var scatter = add.Scatter(
      nodes.Select(node => (double)node.X).ToArray(),
      nodes.Select(node => (double)node.Y).ToArray()
    );

    scatter.LineStyle.Width = 0.01f;
    scatter.Label = $"Liczba punktów: {instance.Nodes.Count}";

    return add;
  }

  public static void Save(this Plot chart, string filename) {
    if (!Directory.Exists($"{ProjectDirectory}/resources/graphs")) Directory.CreateDirectory($"{ProjectDirectory}/resources/graphs");

    chart.SavePng($"{ProjectDirectory}/resources/graphs/{filename}.png", 1200, 800);
  }
}
