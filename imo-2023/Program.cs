// See https://aka.ms/new-console-template for more information

global using System;
using static System.Console;
using static imo_2023.Prelude;

PrintMatrix(Instance.Read("kroA100").Distances);

void PrintMatrix(int[,] matrix)
{
  WriteLine('[');
  for (var i = 0; i < matrix.GetLength(0); i++)
  {
    for (var j = 0; j < matrix.GetLength(1); j++)
    {
      Write(matrix[i, j]);
      if (j < matrix.GetLength(1) - 1) Write(", ");
    }

    WriteLine(';');
  }

  WriteLine(']');
}

internal record Instance(int Dimension, IReadOnlyList<Node> nodes, int[,] Distances)
{
  public static Instance Read(string path)
  {
    var nodes = Node.From(File.ReadLines($"{ProjectDirectory}/resources/instances/{path}.tsp")
      .Skip(6).SkipLast(1)).ToList();

    return new Instance(nodes.Count, nodes, CreateDistanceMatrix(nodes));
  }

  private static int[,] CreateDistanceMatrix(IEnumerable<Node> nodes)
  {
    var items = nodes.ToArray();
    var matrix = new int[items.Length, items.Length];

    for (var i = 0; i < items.Length; i++)
    for (var j = 0; j < items.Length; j++)
      matrix[i, j] = items[i].DistanceTo(items[j]);

    return matrix;
  }
}

internal record Node(int X, int Y)
{
  public static IEnumerable<Node> From(IEnumerable<string> descriptors) =>
    descriptors.Select(descriptor => descriptor.Split(" ")).Select(coords => new Node(
      int.Parse(coords[1]), int.Parse(coords[2])
    ));

  public int DistanceTo(Node other) => (int)Math.Round(Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2)));
}
