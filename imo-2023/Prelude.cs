namespace imo_2023;

internal static class Prelude
{
    public static readonly string ProjectDirectory =
        Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName!;
}
