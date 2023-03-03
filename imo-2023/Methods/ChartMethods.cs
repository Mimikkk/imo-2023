using imo_2023.DataStructures;
using Microsoft.VisualBasic.ApplicationServices;
using ScottPlot;
using ScottPlot.WinForms;

namespace imo_2023.Methods;

internal static class ChartMethods {
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
  public static void View(this FormsPlot form, Plot chart) {
    if (!Directory.Exists($"{ProjectDirectory}/resources/graphs")) Directory.CreateDirectory($"{ProjectDirectory}/resources/graphs");
    form.Reset(chart);
    form.Refresh();
  }
}
