// See https://aka.ms/new-console-template for more information

global using System;
using static System.Console;
using static imo_2023.Prelude;

WriteLine(Instance.Read("kroA100").Dimension);


internal record Instance(int Dimension, IReadOnlyList<Node> nodes)
{
  public static Instance Read(string path)
  {
    var nodes = Node.From(File.ReadLines($"{ProjectDirectory}/resources/instances/{path}.tsp")
      .Skip(6).SkipLast(1)).ToList();

    return new Instance(nodes.Count, nodes);
  }
}

internal record Node(int X, int Y)
{
  public static IEnumerable<Node> From(IEnumerable<string> descriptors) =>
    descriptors.Select(descriptor => descriptor.Split(" ")).Select(coords => new Node(
      int.Parse(coords[1]), int.Parse(coords[2])
    ));
}
