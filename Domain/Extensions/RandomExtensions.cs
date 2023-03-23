namespace Domain.Extensions;

public static class RandomExtensions {
  public static T Choose<T>(this Random random, IList<T> list) => list[random.Next(list.Count)];

  public static T Choose<T>(this Random random, IEnumerable<T> list) {
    var enumerable = list as T[] ?? list.ToArray();
    return enumerable[random.Next(enumerable.Length)];
  }
}
