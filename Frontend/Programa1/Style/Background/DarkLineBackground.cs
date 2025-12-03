using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Background;

public class DarkLineBackground : IBackground{

    public void Render(RenderContext ctx){
        ctx.Display(
            draw: dc => {
                var backgroundColor = Color.Parse("#14256B");
                var backgroundBrush = new SolidColorBrush(backgroundColor);
                dc.FillRectangle(backgroundBrush, new Rect(ctx.Bounds.Size));
                DrawLinePattern(dc, ctx.Bounds);
                var topStripColor = Color.Parse("#2E146B");
                var topStripBrush = new SolidColorBrush(topStripColor);
                double topHeight = ctx.Bounds.Height * 0.01;
                dc.FillRectangle(topStripBrush, new Rect(0, 0, ctx.Bounds.Width, topHeight));

            }
        );

    }
    private void DrawLinePattern(DrawingContext dc, Rect bounds){
        var lineColor = Color.Parse("#6A247B");
        var lineBrush = new SolidColorBrush(lineColor);
        var pen = new Pen(lineBrush, 0.7);

        double spacing = 40.0;
        for(double y = 0; y < bounds.Height; y += spacing){
            dc.DrawLine(pen, new Point(0, y), new Point(bounds.Width, y));
        }
        for(double x = 0; x < bounds.Width; x += spacing){
            dc.DrawLine(pen, new Point(x, 0), new Point(x, bounds.Height));
        }
    }

}
