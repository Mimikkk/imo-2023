using System.Collections;

namespace Domain.Extensions;

public static class EnumerableExtensions {
  public static IEnumerable<T> Yield<T>(params T[] items) => items;
  public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items) => items.SelectMany(x => x);
  public static void Invoke(Action action) => action();

  public static IEnumerable<T>
    Except<T>(this IEnumerable<T> enumerable, params T[] items) => enumerable.Except(items.AsEnumerable());

  public static IEnumerable<(T a, T b)> Pairwise<T>(this IEnumerable<T> source) => Window(source, 2)
    .Select(
      enumerable => {
        var items = enumerable.ToArray();

        return (items[0], items[1]);
      });

  public static IEnumerable<(T a, T b, T c)> Tripletwise<T>(this IEnumerable<T> source) =>
    Window(source, 3)
      .Select(enumerable => {
        var items = enumerable.ToArray();

        return (items[0], items[1], items[2]);
      });

  public static IEnumerable<IEnumerable<T>> Window<T>(this IEnumerable<T> source, int size) {
    var current = new T[size];
    using var it = source.GetEnumerator();

    for (var i = 0; i < size && it.MoveNext(); ++i) current[i] = it.Current;
    while (it.MoveNext()) {
      yield return current;
      current = current.Skip(1).Append(it.Current).ToArray();
    }
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

  public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) {
    foreach (var item in items) action(item);
  }

  public static void ForEach<T, Y>(this IEnumerable<T> items, Action<T, Y?> action) {
    foreach (var item in items) action(item, default);
  }

  public static void ForEach<T>(this IEnumerable<T> items, Func<T, dynamic> action) {
    foreach (var item in items) action(item);
  }

  public static void ForEach<T, Y>(this IEnumerable<T> items, Func<T, Y?, dynamic> action) {
    foreach (var item in items) action(item, default);
  }
}
