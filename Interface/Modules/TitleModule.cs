using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;

namespace Interface.Modules;

internal sealed record TitleModule(MainWindow Self) {
  public void Update() => Self.Title = string.Join(" : ", Updates.Select(a => a()).Where(s => s != ""));
  private readonly List<Func<string>> Updates = new() {
    () => {
      var (mx, my) = Self.Chart.Interaction.GetMouseCoordinates();
      return $"Pozycja Myszy - {(int)mx}x, {(int)my}y";
    },
    () => Self._selectedNode switch {
      var (index, x, y) => $"Wierzchołek - {index + 1} - {x}x, {y}y",
      null              => "",
    },
    () => {
      if (Self._selectedNode is null) return "";
      var contained = Self.Mod.Memory.Histories
        .Where(history => history.Count > Self.Mod.Interaction.Step)
        .FirstOrDefault(x => x[Self.Mod.Interaction.Step].Contains(Self._selectedNode));
      if (contained is null) return "";
      var index = contained[Self.Mod.Interaction.Step].IndexOf(Self._selectedNode);
      return $"Indeks - {index}";
    }
  };
  public void Subscribe(Func<string> update) => Updates.Add(update);
}
