using imo_2023.Algorithms;
using imo_2023.DataStructures;

var instance = Instance.KroA;
var path = instance.PerformGreedyNearestNeighbour();
CreateNodeGraph(nodes: path, filename: "nearest-neighbour");
