namespace Domain.Extensions;

public static class RandomExtensions {
  public static T Choose<T>(this Random random, IEnumerable<T> list) => random.Choose(list.ToArray());
  public static T Choose<T>(this Random random, IList<T> list) => list[random.Next(list.Count)];
}
