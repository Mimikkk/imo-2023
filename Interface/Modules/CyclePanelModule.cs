using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain.Extensions;
using Domain.Structures;
using Interface.Types;
using Microsoft.VisualBasic;
using ScottPlot;
using static Domain.Extensions.EnumerableExtensions;

namespace Interface.Modules;

internal sealed record CyclePanelModule {
  private static class Moves {
    public static void Insert(IList<Node> cycle, Node node, (Node a, Node b) edge) {
      if (edge.a == cycle.First() && edge.b == cycle.Last() || edge.b == cycle.First() && edge.a == cycle.Last()) {
        cycle.Add(node);
        return;
      }


      var ia = cycle.IndexOf(edge.a);
      var ib = cycle.IndexOf(edge.b);
      cycle.Insert(ia > ib ? ia : ib, node);
    }

    public static void ExchangeVertex(IList<Node> first, IList<Node> second, Node a, Node b) {
      var (ia, ib) = (first.IndexOf(a), second.IndexOf(b));
      first.Remove(a);
      first.Insert(ia, b);
      second.Remove(b);
      second.Insert(ib, a);
    }

    public static void ExchangeVertex(IList<Node> cycle, Node a, Node b) => cycle.Swap(a, b);

    public static void ExchangeEdge(IList<Node> cycle, Node a, Node b) {
      var ia = cycle.IndexOf(a);
      var ib = cycle.IndexOf(b);

      if (ia > ib) (ia, ib) = (ib, ia);
      if (ia == 0 && ib == cycle.Count - 1) cycle.Swap(ia, ib);
      if (ia == ib) return;


      while (ia < ib) {
        (cycle[ia], cycle[ib]) = (cycle[ib], cycle[ia]);
        ++ia;
        --ib;
      }
    }
  }

  public CyclePanelModule(MainWindow window) {
    Self = window;

    Self.NodeOperationButton.Click += (_, _) => {
      Operation();

      var first = Mouse.Selection.FirstOrDefault();
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
          var display = $"S: {index}";

          var contained = Cycles.Find(c => c.Contains(node));
          var i = contained?.IndexOf(node) ?? -1;
          if (i != -1) display += $" - C:{Cycles.IndexOf(contained!)}/{i}";
          display += $" - {node.Index}/{node.X}/{node.Y}";

          return new Option<Action> { Name = display, };
        });
      },
      () => {
        var selection = Mouse.Selection;
        var selected = selection.Any() ? Cycles.FirstOrDefault(n => n.Contains(selection)) : null;
        var partiallySelected = Cycles.Where(n => n.ContainsAny(selection)).ToList();

        var options = new List<Option<Action>>()
          .AddWhen(new(
            "Utwórz",
            () => Cycles.Add(selection.ToList())
          ), selection.Count > 2 && !partiallySelected.Any())
          .AddWhen(new(
              "Dodaj Wierzchołek",
              () => Moves.Insert(partiallySelected.First(), selection[0], (selection[1], selection[2]))
            ), selection.Count == 3
               && !partiallySelected.Any(c => c.Contains(selection.First()))
               && partiallySelected.Count == 1
               && partiallySelected.First().NextTo(selection[1], selection[2]))
          .AddWhen(new(
            "Wymień zewnętrzne wierzchołki",
            () => {
              var first = partiallySelected.Find(x => x.Contains(selection[0]))!;
              var second = partiallySelected.Find(x => x.Contains(selection[1]))!;
              var (a, b) = (selection[0], selection[1]);
              Moves.ExchangeVertex(first, second, a, b);
            }
          ), selection.Count == 2 && partiallySelected.Count == 2)
          .AddWhen(new(
            "Wymień Wewnętrzne wierzchołki",
            () => Moves.ExchangeVertex(selected!, selection[0], selection[1])
          ), selection.Count == 2 && selected is not null)
          .AddWhen(new(
            "Wymień wewnętrzne krawędzi",
            () => Moves.ExchangeEdge(selected!, selection[0], selection[1])
          ), selection.Count == 2 && selected is not null)
          .AddWhen(new(
            "Usuń wierzchołek",
            () => {
              if (selected!.Count < 4) Cycles.Remove(selected);
              else selected.Remove(selection.First());
            }), selected is not null && selection.Count == 1)
          .AddWhen(new(
            "Rozwiąż",
            () => Cycles.Remove(selected!)
          ), selected is not null)
          .AddWhen(new(
            "Rozwiąż Wszystkie",
            Cycles.Clear
          ), Cycles.Count > 0 && selection.Count == 0);

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
  public List<List<Node>> Cycles = new();
}
