namespace Algorithms;

internal static class Globals {
  public static readonly string ProjectDirectory =
    Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName!;
}
