using ScottPlot;

namespace Domain.Extensions;

public static class PlotExtensions {
  public static void Save(this Plot chart, string filename) {
    if (!Directory.Exists($"{ProjectDirectory}/Resources/graphs")) Directory.CreateDirectory($"{ProjectDirectory}/Resources/Graphs");

    chart.SavePng($"{ProjectDirectory}/Resources/Graphs/{filename}.png", 1200, 800);
  }
}
