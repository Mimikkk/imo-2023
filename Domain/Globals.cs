namespace Domain;

public static class Globals {
  public static readonly string ProjectDirectory =
    Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName!;

  public static readonly string ResourcesDirectory =
    Path.Combine(ProjectDirectory, "Resources");

  public static Random Random;
}
