using System.Diagnostics;
using System.Numerics;

namespace Algorithms.DataStructures;

public record Instance(int Dimension, List<Node> Nodes, int[,] Distances) {
  public static Instance Read(string name) {
    var nodes = Node.From(File.ReadLines($"{ProjectDirectory}/Resources/Instances/{name}.tsp")
        .Skip(6)
        .SkipLast(1))
      .ToList();

    return new Instance(nodes.Count, nodes, CreateDistanceMatrix(nodes));
  }

  private static int[,] CreateDistanceMatrix(IEnumerable<Node> nodes) {
    var items = nodes.ToArray();
    var distances = new int[items.Length, items.Length];

    for (var i = 0; i < items.Length; ++i)
    for (var j = 0; j < items.Length; ++j) {
      if (i == j) continue;
      distances[i, j] = items[i].DistanceTo(items[j]);
      distances[j, i] = items[j].DistanceTo(items[i]);
    }

    return distances;
  }


  public int this[Node first, Node second] => Distances[first.Index, second.Index];
  public int this[int first, int second] => Distances[first, second];
  public int this[(Node a, Node b) edge] => this[edge.a, edge.b];

  public Node ClosestTo(Node node, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return Nodes.Except(except.Concat(Yield(node))).MinBy(n => this[n, node])!;
  }

  public Node FurthestTo(Node node, IEnumerable<Node>? except = null) {
    except ??= new List<Node>();

    return Nodes.Except(except.Concat(Yield(node))).MaxBy(n => this[n, node])!;
  }

  public int DistanceOf(IEnumerable<Node> cycle) =>
    cycle.Edges().Sum(edge => this[edge]);

  public int InsertCost((Node a, Node b) edge, Node node) =>
    this[edge.a, node] + this[node, edge.b] - this[edge];

  public static bool Approximately(float a, float b) =>
    Math.Abs(b - a) < Math.Max(1E-06f * Math.Max(Math.Abs(a), Math.Abs(b)), float.Epsilon * 8f);

  enum GeometryRelation {
    LeftOf,
    RightOf,
    OnTheLine
  }

  public static float IsAPointLeftOfVectorOrOnTheLine(Node a, Node b, Node p) {
    return (a.X - p.X) * (b.Y - p.Y) - (a.Y - p.Y) * (b.X - p.Y);
  }

  public static List<Node> GetConvexHull(List<Node> points) {
    if (points.Count <= 3) return points;

    //The list with points on the convex hull
    var convexHull = new List<Node>();

    //Step 1. Find the Node with the smallest x coordinate
    //If several have the same x coordinate, find the one with the smallest z
    var startNode = points[0];
    var startPosition = startNode;

    foreach (var point in points.Skip(1)) {
      //Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
      if (point.X >= startPosition.X &&
          (!Approximately(point.X, startPosition.X) || point.Y >= startPosition.Y)) continue;
      startNode = point;
      startPosition = startNode;
    }

    convexHull.Add(startNode);
    points.Remove(startNode);


    //Step 2. Loop to generate the convex hull
    var currentPoint = convexHull[0];

    //Store collinear points here - better to create this list once than each loop
    var collinearPoints = new List<Node>();

    var counter = 0;
    while (true) {
      if (counter == 2) points.Add(convexHull[0]);
      var next = Node.Choose(points);

      var a = currentPoint;
      var b = next;

      //Test if there's a point to the right of ab, if so then it's the new b
      foreach (var node in points) {
        if (node.Equals(next)) continue;

        //Where is c in relation to a-b
        // < 0 -> to the right
        // = 0 -> on the line
        // > 0 -> to the left
        var relation = IsAPointLeftOfVectorOrOnTheLine(a, b, node);

        //Collinear points
        //Cant use exactly 0 because of floating point precision issues
        //This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
        const float accuracy = 0.00001f;

        if (relation < accuracy && relation > -accuracy) collinearPoints.Add(node);
        else if (relation < 0f) {
          next = node;
          b = next;
          //Clear collinear points
          collinearPoints.Clear();
        }
      }


      //If we have collinear points
      if (collinearPoints.Count > 0) {
        collinearPoints.Add(next);

        //Sort this list, so we can add the collinear points in correct order
        collinearPoints = collinearPoints
          .OrderBy(n => Math.Pow(n.X - currentPoint.X, 2) * Math.Pow(n.Y - currentPoint.Y, 2))
          .ToList();

        convexHull.AddRange(collinearPoints);
        currentPoint = collinearPoints.Last();
        points.RemoveAll(collinearPoints.Contains);
        collinearPoints.Clear();
      }
      else {
        convexHull.Add(next);
        points.Remove(next);
        currentPoint = next;
      }

      if (currentPoint.Equals(convexHull.First())) {
        convexHull.RemoveAt(convexHull.Count - 1);
        break;
      }

      counter += 1;
    }

    return convexHull;
  }
}
