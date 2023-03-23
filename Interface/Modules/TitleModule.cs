using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;

namespace Interface.Modules;

internal sealed record TitleModule {
  public TitleModule(MainWindow self) {
    Self = self;
    _updates = new List<Func<string>> {
      () => M.Selection.FirstOrDefault() switch {
        var (index, x, y) => $"Wierzchołek - {index} - {x}x, {y}y",
        null => "",
      },
      () => {
        if (M.Selection.Count < 1) return "";
        var contained = Self.Mod.Memory.Histories
          .Where(history => history.Count > I.Step)
          .FirstOrDefault(x => x[I.Step].Contains(M.Selection.First()));
        if (contained is null) return "";
        var index = contained[I.Step].IndexOf(M.Selection.First());
        return $"Indeks - {index}";
      }
    };
  }

  public void Update() => Self.Title = string.Join(" : ", _updates.Select(a => a()).Where(s => s != ""));

  public void Subscribe(Func<string> update) => _updates.Add(update);

  private readonly List<Func<string>> _updates;

  private readonly MainWindow Self;
  private MouseModule M => Self.Mod.Mouse;
  private InteractionModule I => Self.Mod.Interaction;
}
