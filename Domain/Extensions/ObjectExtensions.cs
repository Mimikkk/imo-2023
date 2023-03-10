namespace Domain.Extensions;

public static class ObjectExtensions {
  public static T As<T>(this object? any) => (T)any!;
}
