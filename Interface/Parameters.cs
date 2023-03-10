namespace Interface;

public sealed partial class MainWindow {
  private int SelectedParameterRegret => (int)ParameterRegret.Value;
  private int? SelectedParameterStartIndex => ParameterStartIndex.Value is 0 ? null : (int)ParameterStartIndex.Value;
  private int SelectedParameterPopulationSize => (int)ParameterPopulationSize.Value;
  private float SelectedParameterWeight => (float)ParameterWeight.Value;
}
