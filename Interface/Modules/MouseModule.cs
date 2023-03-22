using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Structures;
using ScottPlot.Control;

namespace Interface.Modules;

internal sealed record MouseModule(MainWindow Self) {
  public void UpdateClosest() {
    var mouse = CI.GetMouseCoordinates();
    Closest = I.Instance.Nodes.MinBy(node => node.DistanceTo(mouse))!;
  }

  public void UpdateSelected() {
    if (NotCloseEnough) return;

    if (Selection.Contains(Closest)) Selection.Remove(Closest);
    else Selected = Selected == Closest ? null : Closest;
  }

  public void UpdateSelection() {
    if (NotCloseEnough) return;

    if (Selection.Contains(Closest)) Selection.Remove(Closest);
    else Selection.Add(Closest);
  }

  public Node? Closest { get; private set; }
  public Node? Selected { get; private set; }
  public readonly HashSet<Node> Selection = new();

  private InteractionModule I => Self.Mod.Interaction;
  private Interaction CI => Self.Chart.Interaction;
  private bool NotCloseEnough => Closest is null || CI.GetMouseCoordinates().DistanceTo(Closest) >= 125;
}