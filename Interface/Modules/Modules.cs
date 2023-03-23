namespace Interface.Modules;

internal sealed record Modules {
  public Modules(MainWindow self) {
    Interaction = new(self);
    Memory = new(self);
    Chart = new(self);
    Title = new(self);
    Mouse = new(self);
    Panel = new(self);
  }

  public readonly MouseModule Mouse;
  public readonly TitleModule Title;
  public readonly MemoryModule Memory;
  public readonly ChartRendererModule Chart;
  public readonly InteractionModule Interaction;
  public readonly CyclePanelModule Panel;
}