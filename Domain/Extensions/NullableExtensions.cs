namespace Domain.Extensions;

public static class NullableExtensions {
  public static void Let<T>(this T? any, Action<T> action) where T : struct {
    if (any.HasValue) action(any.Value);
  }

  public static void Let<T>(this T? any, Action<T> action) where T : class {
    if (any is not null) action(any);
  }

  public static void Let<T>(this T? any, Func<T, dynamic> action) where T : struct {
    if (any.HasValue) action(any.Value);
  }

  public static void Let<T>(this T? any, Func<T, dynamic> action) where T : class {
    if (any is not null) action(any);
  }

  public static void And(this bool predicate, Action action) {
    if (predicate) action();
  }

  public static void And(this bool? predicate, Action action) =>
    (predicate.HasValue && predicate.Value).And(action);
}