using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Structures;
using Interface.Types;
using ScottPlot;
using static Domain.Extensions.EnumerableExtensions;

namespace Interface.Modules;

internal sealed record NodePanelModule {
  public NodePanelModule(MainWindow window) {
    Self = window;

    Self.NodeOperationButton.Click += (_, _) => {
      Operation();
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
        Self.NodePanelNodes.Items = Mouse.Selection.Select((node, index) => new Option<Action> {
          Name = $"{index} - {node.Index} : {node.X}x, {node.Y}y",
        });
      },
      () => {
        var options = new List<Option<Action>>();
        var count = Mouse.Selection.Count;
        if (count > 2 && Cycle.Count == 0)
          options.Add(new() {
            Name = "Utwórz",
            Value = () => Cycle = Mouse.Selection.ToList(),
          });

        if (Cycle.Count != 0) {
          options.Add(new() {
            Name = "Rozwiąż",
            Value = Cycle.Clear
          });
        }

        if (count == 1 && Cycle.Contains(Mouse.Selected)) {
          options.Add(new() {
            Name = "Usuń",
            Value = () => { },
          });
        }

        Self.NodeOperationButton.IsVisible = options.Count > 0;
        Self.NodeOperations.IsVisible = Self.NodeOperationButton.IsVisible;
        Self.NodeOperations.SelectedIndex = 0;
        Self.NodeOperations.Items = options;
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