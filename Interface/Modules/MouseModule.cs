using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Structures;
using ScottPlot.Control;

namespace Interface.Modules;

internal sealed record MouseModule(MainWindow Self) {
  public void OnMove() {
    var mouse = CI.GetMouseCoordinates();
    ClosestNode = I.Instance.Nodes.MinBy(node => node.DistanceTo(mouse))!;
  }

  public void OnClick() {
    if (!(CI.GetMouseCoordinates().DistanceTo(ClosestNode!) < 125)) return;
    SelectedNode = SelectedNode == ClosestNode ? null : ClosestNode;
  }

  public void OnCtrlClick() {
    if (ClosestNode is null || !(CI.GetMouseCoordinates().DistanceTo(ClosestNode) < 125)) return;
    if (SelectedNodes.Contains(ClosestNode)) SelectedNodes.Remove(ClosestNode);
    else SelectedNodes.Add(ClosestNode);
  }

  public Node? ClosestNode { get; private set; }
  public Node? SelectedNode { get; private set; }
  public readonly HashSet<Node> SelectedNodes = new();

  private InteractionModule I => Self.Mod.Interaction;
  private Interaction CI => Self.Chart.Interaction;
}