using ScottPlot;
using ScottPlot.Axis;
using SkiaSharp;

namespace Domain.Structures;

public sealed class Annotation : IPlottable {
  public Annotation(Coordinates coordinate, string text) {
    Text = text;
    Coordinate = coordinate;
  }

  public Annotation()
    : this(Coordinates.NaN, string.Empty) { }

  public void Render(SKSurface surface) {
    using var paint = new SKPaint { IsAntialias = true, TextAlign = SKTextAlign.Center };
    var canvas = surface.Canvas;
    var width = paint.MeasureText(Text);
    var pixel = Axes.GetPixel(Coordinate);

    paint.Color = Color;
    canvas.DrawRoundRect(pixel.X - width / 2 - 4, pixel.Y - paint.TextSize - 2, width + 8, paint.TextSize + 8, 2, 2, paint);
    paint.Color = SKColors.Wheat;
    canvas.DrawText(Text, pixel.X, pixel.Y, paint);
  }

  public AxisLimits GetAxisLimits() => new();
  public bool IsVisible { get; set; } = true;
  public IAxes Axes { get; set; } = ScottPlot.Axis.Axes.Default;
  public IEnumerable<LegendItem> LegendItems { get; } = Yield(new LegendItem());
  public string Text { get; set; }
  public Coordinates Coordinate { get; set; }
  public SKColor Color { get; set; } = SKColors.Navy;
}
