using System.Linq;
using Algorithms.Algorithms;
using Algorithms.DataStructures;
using Algorithms.Extensions;
using Interface.Types;

namespace Interface;

public partial class MainWindow {
  private int SelectedParameterRegret => (int)ParameterRegret.Value;
  private int SelectedParameterStartIndex => (int)ParameterStartIndex.Value;
  private int SelectedParameterPopulationSize => (int)ParameterPopulationSize.Value;
}
