using imo_2023.Extensions;

namespace imo_2023.Methods;

public static class LogMethods {
  public static void W(object? any) => Write(any);
  public static void Wl(object? any) => WriteLine(any);

  public static void W<T>(T[,] matrix) {
    W('[');
    for (var i = 0; i < matrix.GetLength(0); i++) {
      W(matrix.ReadRow(i));
      if (i < matrix.GetLength(0) - 1) Wl(", ");
    }
    W(']');
  }
  public static void Wl<T>(T[,] matrix) {
    W(matrix);
    WriteLine();
  }

  public static void W<T>(IEnumerable<T> enumerable) {
    Write('[');
    var collected = enumerable.ToArray();

    for (var i = 0; i < collected.Length; i++) {
      W(collected[i]!);
      if (i < collected.Length - 1) Write(", ");
    }
    Write(']');
  }
  public static void Wl<T>(IEnumerable<T> enumerable) {
    W(enumerable);
    WriteLine();
  }

  public static void W<T>(HashSet<T> set) {
    Write('{');
    var collected = set.ToArray();

    for (var i = 0; i < collected.Length; i++) {
      W(collected[i]!);
      if (i < collected.Length - 1) Write(", ");
    }
    Write('}');
  }
  public static void Wl<T>(HashSet<T> set) {
    W(set);
    WriteLine();
  }
}
