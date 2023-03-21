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

  public Node? ClosestNode { get; private set; }
  public Node? SelectedNode { get; private set; }

  private InteractionModule I => Self.Mod.Interaction;
  private Interaction CI => Self.Chart.Interaction;
}
