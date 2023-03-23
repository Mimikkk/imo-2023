﻿using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;
using ScottPlot;
using SkiaSharp;
using static Domain.Extensions.EnumerableExtensions;

namespace Interface.Modules;

internal sealed record ChartRendererModule {
  public void Notify() {
    Self.Chart.Plot.Clear();
    Updates.ForEach(Invoke);
    Self.Chart.Refresh();
  }

  public ChartRendererModule(MainWindow window) {
    Self = window;
    Updates = new() {
      () => Add.Scatter(I.Instance.Nodes),
      () => M.Histories.ToList().ForEach(Render),
      () => (P.Cycle.Count > 0).And(() => Add.Cycle(P.Cycle, I.Instance)),
      () => Mouse.Closest.Let(Add.Point),
      () => {
        if (Mouse.Selected is null) return;
        Add.Point(Mouse.Selected, SKColors.Coral);

        var color = Self.Chart.Plot.Plottables.Count;
        var plotted = new List<Node>();

        foreach (var history in M.Histories.Where(history => history.Count > I.Step)) {
          var nodes = history[I.Step].Except(Yield(Mouse.Selected)).ToList();

          plotted.AddRange(nodes);
          Add.DistanceTo(Mouse.Selected, nodes, M.Palette.GetColor(++color).ToSKColor());
        }

        Add.DistanceTo(Mouse.Selected,
          I.Instance.Nodes.Except(plotted)
            .Except(Mouse.Selection)
            .Except(Yield(Mouse.Selected)));
      },
      () => Mouse.Selection.ForEach(Add.Point),
      () => M.Average.Let(average => Add.Label($"Przeciętna długość: {average:F2}")),
      () => (I.Parameter.PopulationSize > 1).And(() =>
        Add.Label($"Łączna długość: {
          M.Histories.Sum(history =>
            I.Instance.DistanceOf(history.ElementAtOrDefault(I.Step) ?? history[^1]))
        }"))
    };
  }

  private void Render(IEnumerable<IEnumerable<Node>> history) {
    var enumerable = history as IEnumerable<Node>[] ?? history.ToArray();
    var step = enumerable.ElementAtOrDefault(I.Step) ?? enumerable[^1];
    Render(step);
  }

  private void Render(IEnumerable<Node> cycle) {
    switch (I.Algorithm.DisplayAs) {
      case Algorithm.DisplayType.Cycle: {
        Add.Cycle(cycle, I.Instance);
        return;
      }
      case Algorithm.DisplayType.Path: {
        if (I.Step == (int)Self.HistorySlider.Maximum) Add.Cycle(cycle, I.Instance);
        else Add.Path(cycle, I.Instance);
        return;
      }
    }
  }

  private MainWindow Self { get; }
  private AddPlottable Add => Self.Chart.Plot.Add;
  private readonly List<Action> Updates;
  private InteractionModule I => Self.Mod.Interaction;
  private MemoryModule M => Self.Mod.Memory;
  private MouseModule Mouse => Self.Mod.Mouse;
  private CyclePanelModule P => Self.Mod.Panel;
  public void Subscribe(Action update) => Updates.Add(update);
}