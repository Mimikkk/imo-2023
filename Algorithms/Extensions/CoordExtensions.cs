using Algorithms.DataStructures;
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
  public static double DistanceTo(this Coordinates coord, Node other) {
    var (x, y) = coord;
    var (ox, oy) = other;
    return Math.Sqrt(Math.Pow(x - ox, 2) + Math.Pow(y - oy, 2));
  }

  public static bool InBounds(this (int X, int Y) coord, int width, int height) {
    var (x, y) = coord;
    return x >= 0 && x < width && y >= 0 && y < height;
  }
  public static void Deconstruct(this Node coord, out int x, out int y) {
    x = coord.X;
    y = coord.Y;
  }
  public static void Deconstruct(this Node coord, out int index, out int x, out int y) {
    index = coord.Index;
    (x, y) = coord;
  }

  public static bool InBounds(this Node coord, int width, int height) {
    var (x, y) = coord;
    return x >= 0 && x < width && y >= 0 && y < height;
  }
  public static bool InBounds(this Node coord, double minx, double maxx, double miny, double maxy) {
    var (x, y) = coord;
    return x >= minx && x < maxx && y >= miny && y < maxy;
  }
}
