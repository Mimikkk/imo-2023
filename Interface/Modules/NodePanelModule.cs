using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Interface.Types;
using ScottPlot;
using static Domain.Extensions.EnumerableExtensions;

namespace Interface.Modules;

internal sealed record NodePanelModule {
  public NodePanelModule(MainWindow window) {
    Self = window;
    Updates = new() {
      () => {
        var (mx, my) = Self.Chart.Interaction.GetMouseCoordinates();
        Self.TextMousePosition.Text = $"Mysz - {(int)mx}x, {(int)my}y";
      },
      () => {
        Self.NodePanelDescription.IsVisible = Mouse.Selection.Count > 0;
        Self.NodePanelNodes.IsVisible = Self.NodePanelDescription.IsVisible;
        Self.NodePanelNodes.Items = Mouse.Selection.Select((node, index) => new Option {
          Name = $"{index} - {node.Index} : {node.X}x, {node.Y}y",
        });
      }
    };
  }

  public void Notify() => Updates.ForEach(Invoke);

  private MainWindow Self { get; }
  private AddPlottable Add => Self.Chart.Plot.Add;
  private readonly List<Action> Updates;
  private InteractionModule I => Self.Mod.Interaction;
  private MemoryModule M => Self.Mod.Memory;
  private MouseModule Mouse => Self.Mod.Mouse;
  public void Subscribe(Action update) => Updates.Add(update);
}