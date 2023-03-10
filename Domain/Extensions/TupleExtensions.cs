namespace Domain.Extensions;

public static class TupleExtensions {
  public static (T1, T2) DropLast<T1, T2, T3>(this (T1, T2, T3) _) => (_.Item1, _.Item2);
}
