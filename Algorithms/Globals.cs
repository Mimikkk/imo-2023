namespace imo_2023;

internal static class Globals {
  public static readonly string ProjectDirectory =
    Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName!;
}
