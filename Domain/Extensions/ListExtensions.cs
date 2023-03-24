namespace Domain.Extensions;

public static class ListExtensions {
  public static void Swap<T>(this IList<T> list, int i, int j) => (list[i], list[j]) = (list[j], list[i]);
  public static void Swap<T>(this IList<T> list, T a, T b) => list.Swap(list.IndexOf(a), list.IndexOf(b));

  public static bool Contains<T>(this IList<T> list, IEnumerable<T> items) => items.All(list.Contains);
  public static bool Contains<T>(this IList<T> list, IEnumerable<IEnumerable<T>> items) => items.All(list.Contains);
  public static bool Contains<T>(this IList<T> list, params T[] items) => items.All(list.Contains);
  public static bool Contains<T>(this IList<T> list, params T[][] items) => items.All(list.Contains);

  public static bool ContainsAny<T>(this IList<T> list, IEnumerable<T> items) => items.Any(list.Contains);
  public static bool ContainsAny<T>(this IList<T> list, IEnumerable<IEnumerable<T>> items) => items.Any(list.Contains);
  public static bool ContainsAny<T>(this IList<T> list, params T[] items) => items.Any(list.Contains);
  public static bool ContainsAny<T>(this IList<T> list, params T[][] items) => items.Any(list.Contains);
}
