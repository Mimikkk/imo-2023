﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;

namespace Interface.Modules;

internal sealed record TitleModule {
  public TitleModule(MainWindow self) {
    Self = self;
    Updates = new List<Func<string>> {
      () => {
        var (mx, my) = Self.Chart.Interaction.GetMouseCoordinates();
        return $"Pozycja Myszy - {(int)mx}x, {(int)my}y";
      },
      () => M.Selected switch {
        var (index, x, y) => $"Wierzchołek - {index + 1} - {x}x, {y}y",
        null              => "",
      },
      () => {
        if (M.Selected is null) return "";
        var contained = Self.Mod.Memory.Histories
          .Where(history => history.Count > I.Step)
          .FirstOrDefault(x => x[I.Step].Contains(M.Selected));
        if (contained is null) return "";
        var index = contained[I.Step].IndexOf(M.Selected);
        return $"Indeks - {index}";
      }
    };
  }

  public void Update() => Self.Title = string.Join(" : ", Updates.Select(a => a()).Where(s => s != ""));

  public void Subscribe(Func<string> update) => Updates.Add(update);

  private readonly List<Func<string>> Updates;

  private readonly MainWindow Self;
  private MouseModule M => Self.Mod.Mouse;
  private InteractionModule I => Self.Mod.Interaction;
}