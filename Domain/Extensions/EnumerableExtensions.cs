﻿namespace Domain.Extensions;

public static class EnumerableExtensions {
  public static IEnumerable<T> Yield<T>(params T[] items) => items;
  public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items) => items.SelectMany(x => x);
  public static void Invoke(Action action) => action();
  public static IEnumerable<T> Times<T>(int count, Func<T> action) => Enumerable.Range(0, count).Select(_ => action());

  public static IEnumerable<T>
    Except<T>(this IEnumerable<T> enumerable, params T[] items) => enumerable.Except(items.AsEnumerable());

  public static IEnumerable<(T a, T b)> Pairwise<T>(this IEnumerable<T> source) =>
    Window(source, 2).Select(enumerable => enumerable.First2());

  public static IEnumerable<(T a, T b, T c)> TripletWise<T>(this IEnumerable<T> source) =>
    Window(source, 3).Select(enumerable => enumerable.First3());

  public static IEnumerable<IEnumerable<T>> Window<T>(this IEnumerable<T> source, int size) {
    var current = new T[size];
    using var it = source.GetEnumerator();

    for (var i = 0; i < size && it.MoveNext(); ++i) current[i] = it.Current;
    while (it.MoveNext()) {
      yield return current;
      current = current.Skip(1).Append(it.Current).ToArray();
    }
  }

  public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> items, int k) => Combinations(items.ToArray(), k);
  public static IEnumerable<IEnumerable<T>> Combinations<T>(this IList<T> items, int k) {
    if (k == 0) return Yield(Enumerable.Empty<T>());
    if (!items.Any()) return Enumerable.Empty<IEnumerable<T>>();

    var head = items.First();
    var tail = items.Skip(1).ToArray();
    return tail.Combinations(k - 1).Select(x => new[] { head }.Concat(x)).Concat(tail.Combinations(k));
  }

  public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) => items.OrderBy(_ => Globals.Random.Next());

  public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) {
    foreach (var item in items) action(item);
  }

  public static void ForEach<T, Y>(this IEnumerable<T> items, Func<T, Y> action) {
    foreach (var item in items) action(item);
  }


  public static (T a, T b) First2<T>(this IEnumerable<T> items) {
    using var it = items.GetEnumerator();
    it.MoveNext();
    var a = it.Current;
    it.MoveNext();
    var b = it.Current;

    return (a, b);
  }
  public static (T a, T b) First2<T>(this IList<T> items) => (items[0], items[1]);

  public static (T a, T b, T c) First3<T>(this IEnumerable<T> items) {
    using var it = items.GetEnumerator();
    it.MoveNext();
    var a = it.Current;
    it.MoveNext();
    var b = it.Current;
    it.MoveNext();
    var c = it.Current;

    return (a, b, c);
  }
  public static (T a, T b, T c) First3<T>(this IList<T> items) => (items[0], items[1], items[2]);
}
