using Avalonia.Controls;
using ScottPlot.Avalonia;

namespace Interface;

public partial class MainWindow : Window {

  public MainWindow() {
    InitializeComponent();

    var dataX = new double[] { 1, 2, 3, 4, 5, 6, 7, 8 };
    var dataY = new double[] { 1, 4, 9, 16, 25, 35, 40 };
    Chart.Plot.Add.Scatter(dataX, dataY);
    Chart.Refresh();
  }
}
