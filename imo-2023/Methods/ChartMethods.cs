using imo_2023.DataStructures;
using ScottPlot;

namespace imo_2023.Methods;

internal static class ChartMethods {
  public static void CreateNodeGraph(IEnumerable<Node> nodes, string filename) {
    var chart = new Plot();
    var enumerable = nodes as Node[] ?? nodes.ToArray();

    var dataX = enumerable.Select(node => (double)node.X).Append(enumerable.First().X).ToArray();
    var dataY = enumerable.Select(node => (double)node.Y).Append(enumerable.First().Y).ToArray();

    chart.Add.Scatter(dataX, dataY);
    SaveToDisk(chart, filename);
  }

  private static void SaveToDisk(Plot chart, string filename) => chart.SavePng($"{ProjectDirectory}/resources/graphs/{filename}.png", 1200, 800);
}
