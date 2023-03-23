using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Structures;
using Interface.Types;
using ScottPlot;
using static Domain.Extensions.EnumerableExtensions;

namespace Interface.Modules;

internal sealed record CyclePanelModule {
  public void Insert(IList<Node> cycle, Node node, (Node a, Node b) edge) {
    var index = cycle.IndexOf(edge.a) > cycle.IndexOf(edge.b) ? cycle.IndexOf(edge.a) : cycle.IndexOf(edge.b);
    cycle.Insert(index, node);
  }

  public CyclePanelModule(MainWindow window) {
    Self = window;

    Self.NodeOperationButton.Click += (_, _) => {
      Operation();

      var first = Mouse.Selected;
      Mouse.Selection.Clear();
      first.Let(Mouse.Selection.Add);

      Notify();
      Self.Mod.Chart.Notify();
    };

    Updates = new() {
      () => {
        var (mx, my) = Self.Chart.Interaction.GetMouseCoordinates();
        Self.TextMousePosition.Text = $"Mysz - {(int)mx}x, {(int)my}y";
      },
      () => {
        Self.NodePanelDescription.IsVisible = Mouse.Selection.Count > 0;
        Self.NodePanelNodes.IsVisible = Self.NodePanelDescription.IsVisible;
        Self.NodePanelNodes.Items = Mouse.Selection.Select((node, index) => {
          var contained = Cycle.IndexOf(node);
          var display = $"S: {index}";
          if (contained != -1) display += $" - C: {contained}";
          display += $" - {node.Index}/{node.X}/{node.Y}";
          return new Option<Action> { Name = display, };
        });
      },
      () => {
        var count = Mouse.Selection.Count;
        var options = new List<Option<Action>>()
          .AddWhen(new(
            "Utwórz",
            () => Cycle = Mouse.Selection.ToList()
          ), count > 2 && Cycle.Count == 0)
          .AddWhen(new(
            "Dodaj",
            () => Insert(Cycle, Mouse.Selection[0], (Mouse.Selection[1], Mouse.Selection[2]))
          ), count == 3 && !Cycle.Contains(Mouse.Selected!) && Cycle.Contains(Mouse.Selection[1], Mouse.Selection[2]))
          .AddWhen(new(
            "Usuń",
            () => Cycle.Remove(Mouse.Selected!)
          ), count == 1 && Cycle.Count > 2 && Mouse.Selected is not null && Cycle.Contains(Mouse.Selected))
          .AddWhen(new(
            "Rozwiąż",
            Cycle.Clear
          ), Cycle.Count != 0);

        Self.NodeOperationButton.IsVisible = options.Count > 0;
        Self.NodeOperations.IsVisible = Self.NodeOperationButton.IsVisible;
        Self.NodeOperations.Items = options;
        Self.NodeOperations.SelectedIndex = 0;
      }
    };
  }

  public void Notify() => Updates.ForEach(Invoke);

  private Action Operation => Self.NodeOperations.SelectedItem.As<Option<Action>>().Value;
  private MainWindow Self { get; }
  private AddPlottable Add => Self.Chart.Plot.Add;
  private readonly List<Action> Updates;
  private InteractionModule I => Self.Mod.Interaction;
  private MemoryModule M => Self.Mod.Memory;
  private MouseModule Mouse => Self.Mod.Mouse;
  public void Subscribe(Action update) => Updates.Add(update);
  public List<Node> Cycle = new();
}