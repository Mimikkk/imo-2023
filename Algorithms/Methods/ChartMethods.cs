using Algorithms.DataStructures;
using ScottPlot;
using ScottPlot.Extensions;
using SkiaSharp;

namespace Algorithms.Methods;

public static class ChartMethods {
  public static AddPlottable Cycle(this AddPlottable add, IEnumerable<Node> cycle, Instance instance) {
    var nodes = cycle as Node[] ?? cycle.ToArray();
    if (nodes.Length < 1) return add;

    var scatter = add.Scatter(
      xs: nodes.Select(node => (double)node.X).Append(nodes.First().X).ToArray(),
      ys: nodes.Select(node => (double)node.Y).Append(nodes.First().Y).ToArray()
    );
    scatter.Label = $"Długość cyklu: {instance.DistanceOf(nodes.Append(nodes.First()))}";

    return add;
  }

  public static AddPlottable Path(this AddPlottable add, IEnumerable<Node> cycle, Instance instance) {
    var nodes = cycle as Node[] ?? cycle.ToArray();
    if (nodes.Length < 1) return add;

    var scatter = add.Scatter(
      xs: nodes.Select(node => (double)node.X).ToArray(),
      ys: nodes.Select(node => (double)node.Y).ToArray()
    );
    scatter.Label = $"Długość ścieżki: {instance.DistanceOf(nodes.Append(nodes.First()))}";

    return add;
  }

  public static AddPlottable Scatter(this AddPlottable add, IEnumerable<Node> nodes) {
    nodes = nodes.ToArray();

    var scatter = add.Scatter(
      nodes.Select(node => (double)node.X).ToArray(),
      nodes.Select(node => (double)node.Y).ToArray()
    );

    scatter.LineStyle.Width = 0.01f;
    scatter.Label = $"Liczba punktów: {nodes.Count()}";

    return add;
  }

  public static AddPlottable Point(this AddPlottable add, Node node) {
    var scatter = add.Scatter(
      xs: new[] { (double)node.X },
      ys: new[] { (double)node.Y }
    );
    scatter.MarkerStyle = new MarkerStyle(MarkerShape.OpenCircle, 10f);
    return add;
  }


  public static AddPlottable DistanceTo(this AddPlottable add, Node from, Node to, SKColor? color = null) {
    add.Plottable(new Annotation {
      Coordinate = to,
      Text = $"{from.DistanceTo(to)}",
      Color = color ?? SKColors.Navy
    });
    return add;
  }
  public static AddPlottable DistanceTo(this AddPlottable add, Node from, IEnumerable<Node> to, SKColor? color = null) {
    foreach (var destination in to) add.DistanceTo(from, to: destination, color);
    return add;
  }

  public static (double minx, double maxx, double miny, double maxy) Bounds(this Plot chart)
    => (chart.XAxis.Min, chart.XAxis.Max, chart.YAxis.Min, chart.YAxis.Max);


  public static AddPlottable LineTo(this AddPlottable add, Node from, Node to) {
    var scatter = add.Scatter(
      xs: new double[] { from.X, to.X },
      ys: new double[] { from.Y, to.Y }
    );
    scatter.LineStyle.Pattern = LinePattern.Dash;
    add.DistanceTo(from, to);

    return add;
  }
  public static AddPlottable LineTo(this AddPlottable add, Node from, IEnumerable<Node> to) {
    foreach (var destination in to) add.LineTo(from, to: destination);

    return add;
  }

  public static void Save(this Plot chart, string filename) {
    if (!Directory.Exists($"{ProjectDirectory}/resources/graphs")) Directory.CreateDirectory($"{ProjectDirectory}/resources/graphs");

    chart.SavePng($"{ProjectDirectory}/resources/graphs/{filename}.png", 1200, 800);
  }
}
