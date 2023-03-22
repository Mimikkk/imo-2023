using ScottPlot;
using SkiaSharp;

namespace Domain.Extensions;

public static class ColorExtensions {
  public static Color Into(this SKColor color) =>
    new(color.Red, color.Green, color.Blue, color.Alpha);
}