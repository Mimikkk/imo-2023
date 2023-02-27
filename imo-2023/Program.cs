using imo_2023.Algorithms;
using imo_2023.DataStructures;

var instance = Instance.KroA;
var (first, second) = instance.SearchWithDoubleGreedyNearestNeighbour();

var chart = CreateChart();
chart.Add.Cycle(first, instance).Cycle(second, instance);
chart.Title.Label.Text = "KroA100 - Greedy Nearest Neighbour";
chart.Save("kroA100-greedy-nearest-neighbour");
