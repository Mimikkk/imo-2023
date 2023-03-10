namespace Domain.Extensions;

public static class TupleExtensions {
  public static T1 DropLast<T1, T2>(this (T1, T2) _) => _.Item1;
  public static (T1, T2) DropLast<T1, T2, T3>(this (T1, T2, T3) _) => (_.Item1, _.Item2);
  public static (T1, T2, T3) DropLast<T1, T2, T3, T4>(this (T1, T2, T3, T4) _) => (_.Item1, _.Item2, _.Item3);
}
