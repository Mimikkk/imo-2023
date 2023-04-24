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
    if (NotCloseEnough || !Closest.HasValue) return;

    if (Selection.Contains(Closest.Value)) {
      if (Selection.First() == Closest) Selection.Remove(Closest.Value);
      else Selection.Swap(Selection[0], Closest.Value);
    } else {
      Selection.Clear();
      Selection.Add(Closest.Value);
    }
  }

  public void UpdateSelection() {
    if (NotCloseEnough || !Closest.HasValue) return;

    if (Selection.Contains(Closest.Value)) Selection.Remove(Closest.Value);
    else Selection.Add(Closest.Value);
  }

  public Node? Closest { get; private set; }
  public readonly ObservableList<Node> Selection = new();

  private InteractionModule I => Self.Mod.Interaction;
  private Interaction CI => Self.Chart.Interaction;
  private bool NotCloseEnough => Closest is null || CI.GetMouseCoordinates().DistanceTo(Closest.Value) >= 125;
}
