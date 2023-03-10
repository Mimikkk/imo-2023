using ScottPlot;

namespace Domain.Extensions;

public static class PlotExtensions {
  public static void Save(this Plot chart, string filename) {
    if (!Directory.Exists($"{ProjectDirectory}/resources/graphs")) Directory.CreateDirectory($"{ProjectDirectory}/resources/graphs");

    chart.SavePng($"{ProjectDirectory}/resources/graphs/{filename}.png", 1200, 800);
  }
}
