namespace imo_2023.Extensions;

public static class IEnumerableExtensions {
  public static IEnumerable<T> Yield<T>(T item) {
    yield return item;
  }

  public static IEnumerable<(T a, T b)> Pairwise<T>(this IEnumerable<T> source) {
    var previous = default(T);
    using var it = source.GetEnumerator();

    if (it.MoveNext()) previous = it.Current;
    while (it.MoveNext()) yield return (previous!, previous = it.Current);
  }
}
