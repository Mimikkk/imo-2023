using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;

namespace Interface;

public sealed partial class MainWindow {
  private record TitleControl(MainWindow Self) {
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
        var contained = Self.Memory.Histories
          .Where(history => history.Count > Self.Interaction.Step)
          .FirstOrDefault(x => x[Self.Interaction.Step].Contains(Self._selectedNode));
        if (contained is null) return "";
        var index = contained[Self.Interaction.Step].IndexOf(Self._selectedNode);
        return $"Indeks - {index}";
      }
    };
  }

  private readonly TitleControl TitleController;
}
