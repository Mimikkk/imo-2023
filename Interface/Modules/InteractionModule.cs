using System.Collections.Generic;
using System.Linq;
using Algorithms.Structures;
using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.Instances;
using Interface.Structures;

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

  public Algorithm Algorithm => Self.Algorithms.SelectedItem.As<Option<Algorithm>>().Value;
  public int Step => (int)Self.HistorySlider.Value;

  internal sealed record Parameters(MainWindow Self) {
    public int Regret => (int)Self.ParameterRegret.Value;
    public int? StartIndex => Self.ParameterStartIndex.Value is 0 ? null : (int)Self.ParameterStartIndex.Value;
    public int PopulationSize => (int)Self.ParameterPopulationSize.Value;
    public float Weight => (float)Self.ParameterWeight.Value;
    public float TimeLimit => (float)Self.ParameterTimeLimit.Value;
    public string Variant => Self.ParameterVariants.SelectedItem.As<Option<string>>().Value;

    public Algorithm Initializer => Self.ParameterInitializers.SelectedItem.As<Option<Algorithm>>().Value;

    public SearchConfiguration Configuration => new() {
      Population = Times(PopulationSize, ObservableList<Node>.Create).ToList(),
      Regret = Regret,
      Start = StartIndex,
      Weight = Weight,
      TimeLimit = TimeLimit,
      Initializer = Initializer,
      Variant = Variant
    };
  }

  public readonly Parameters Parameter = new(Self);

  private Instance? _instance;
  private IEnumerable<Node>? _hull;
  private string InstanceSelection => Self.Instances.SelectedItem.As<Option<string>>().Value;
}
