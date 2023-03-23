using System.Collections.Generic;
using System.Linq;
using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;
using Interface.Types;
using static System.Linq.Enumerable;

namespace Interface.Modules;

internal sealed record InteractionModule(MainWindow Self) {
  public Instance Instance {
    get {
      if (_instance is not null && _instance.Name == InstanceSelection) return _instance;
      _instance = Instance.Read(InstanceSelection);
      _hull = null;
      return _instance;
    }
  }

  public IEnumerable<Node> Hull => _hull ??= Instance.Nodes.Hull();

  public Algorithm Algorithm => Algorithm.FromName(Self.Algorithms.SelectedItem.As<Option<string>>().Value);
  public int Step => (int)Self.HistorySlider.Value;

  internal sealed record Parameters(MainWindow Self) {
    public int Regret => (int)Self.ParameterRegret.Value;
    public int? StartIndex => Self.ParameterStartIndex.Value is 0 ? null : (int)Self.ParameterStartIndex.Value;
    public int PopulationSize => (int)Self.ParameterPopulationSize.Value;
    public float Weight => (float)Self.ParameterWeight.Value;
    public float TimeLimit => (float)Self.ParameterTimeLimit.Value;

    public SearchConfiguration Configuration => new() {
      Population = Range(0, PopulationSize).Select(_ => new List<Node>()),
      Regret = Regret,
      Start = StartIndex,
      Weight = Weight,
      TimeLimit = TimeLimit
    };
  }

  public readonly Parameters Parameter = new(Self);

  private Instance? _instance;
  private IEnumerable<Node>? _hull;
  private string InstanceSelection => Self.Instances.SelectedItem.As<Option<string>>().Value;
}
