using ScottPlot;

namespace Algorithms.Extensions;

public static class CoordExtensions {
  public static void Deconstruct(this Coordinates coord, out double x, out double y) {
    x = coord.X;
    y = coord.Y;
  }

  public static double DistanceTo(this Coordinates coord, Coordinates other) {
    var (x, y) = coord;
    var (ox, oy) = other;
    return Math.Sqrt(Math.Pow(x - ox, 2) + Math.Pow(y - oy, 2));
  }
}
