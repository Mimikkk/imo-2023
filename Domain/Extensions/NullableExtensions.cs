namespace Domain.Extensions;

public static class NullableExtensions {
  public static void Let<T>(this T? any, Action<T> action) {
    if (any is not null) action(any);
  }

  public static void Let<T, Y>(this T? any, Func<T, Y> action) where T : struct {
    if (any.HasValue) action(any.Value);
  }

  public static void Let<T, Y>(this T? any, Func<T, Y> action) where T : class {
    if (any is not null) action(any);
  }

  public static T Also<T>(this T any, Action<T> action, bool predicate = true) {
    predicate.And(() => action(any));
    return any;
  }

  public static T Also<T, Y>(this T any, Func<T, Y> action, bool predicate = true) {
    predicate.And(() => action(any));
    return any;
  }

  public static bool And(this bool predicate, Action action) {
    if (predicate) action();
    return predicate;
  }

  public static bool And(this bool? predicate, Action action) =>
    (predicate.HasValue && predicate.Value).And(action);

  public static bool Or(this bool predicate, Action action) {
    if (!predicate) action();
    return predicate;
  }

  public static bool Or(this bool? predicate, Action action) =>
    (predicate.HasValue && !predicate.Value).Or(action);
}
