namespace Domain.Extensions;

public static class EnumerableExtensions {
  public static IEnumerable<T> Yield<T>(params T[] items) => items;
  public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items) => items.SelectMany(x => x);

  public static IEnumerable<(T a, T b)> Pairwise<T>(this IEnumerable<T> source) {
    var previous = default(T);
    using var it = source.GetEnumerator();

    if (it.MoveNext()) previous = it.Current;
    while (it.MoveNext()) yield return (previous!, previous = it.Current);
  }

  public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> items, int count) {
    var enumerable = items as T[] ?? items.ToArray();
    if (count == 1)
      foreach (var item in enumerable)
        yield return Yield(item);
    if (count > enumerable.Length)
      yield return enumerable;

    foreach (var (element, i) in enumerable.Select((item, index) => (item, index + 1))) {
      foreach (var result in Combinations(enumerable.Skip(i), count - 1))
        yield return Yield(element).Concat(result);
    }
  }
}
