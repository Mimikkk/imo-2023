using System.Linq;
using Algorithms.DataStructures;
using Algorithms.Extensions;

namespace Interface;

public partial class MainWindow {
  private void UpdateClosestNode() {
    var mouse = Chart.Interaction.GetMouseCoordinates();
    _closestNode = _instance.Nodes.MinBy(node => node.DistanceTo(mouse))!;
  }

  private void UpdateSelectedNode() {
    if (!(Chart.Interaction.GetMouseCoordinates().DistanceTo(_closestNode!) < 125)) return;
    _selectedNode = _selectedNode == _closestNode ? null : _closestNode;
  }

  private Node? _closestNode;
  private Node? _selectedNode;
}
