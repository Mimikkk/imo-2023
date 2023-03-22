using Domain.Structures;
using ScottPlot;

namespace Domain.Extensions;

public static class ListExtensions {
  public static void Swap<T>(this IList<T> list, int i, int j) => (list[i], list[j]) = (list[j], list[i]);
  public static void Swap<T>(this IList<T> list, T a, T b) => list.Swap(list.IndexOf(a), list.IndexOf(b));
}
