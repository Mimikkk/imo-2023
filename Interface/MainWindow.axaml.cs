using Avalonia.Controls;
using ScottPlot.Avalonia;

namespace Interface;

public partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();

    var dataX = new double[] { 1, 2, 3, 4, 5 };
    var dataY = new double[] { 1, 4, 9, 16, 25 };
    var avaPlot1 = this.Find<AvaPlot>("Plot");
    avaPlot1.Plot.Add.Scatter(dataX, dataY);
    avaPlot1.Refresh();
  }
}
