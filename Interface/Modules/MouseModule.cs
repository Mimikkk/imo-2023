using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Structures;
using Interface.Types;
using ScottPlot.Control;

namespace Interface.Modules;

internal sealed record MouseModule(MainWindow Self) {
  public void UpdateClosest() {
    var mouse = CI.GetMouseCoordinates();
    Closest = I.Instance.Nodes.MinBy(node => node.DistanceTo(mouse))!;
  }

  public void UpdateSelected() {
    if (NotCloseEnough) return;

    if (Selection.Contains(Closest)) {
      if (Selection.First() == Closest) Selection.Remove(Closest);
      else Selection.Swap(Selection[0], Closest);
    }
    else {
      Selection.Clear();
      Selection.Add(Closest);
    }
  }

  public void UpdateSelection() {
    if (NotCloseEnough) return;

    if (Selection.Contains(Closest)) Selection.Remove(Closest);
    else Selection.Add(Closest);
  }

  public Node? Closest { get; private set; }
  public Node? Selected => Selection.FirstOrDefault();
  public readonly ObservableList<Node> Selection = new();

  private InteractionModule I => Self.Mod.Interaction;
  private Interaction CI => Self.Chart.Interaction;
  private bool NotCloseEnough => Closest is null || CI.GetMouseCoordinates().DistanceTo(Closest) >= 125;
}