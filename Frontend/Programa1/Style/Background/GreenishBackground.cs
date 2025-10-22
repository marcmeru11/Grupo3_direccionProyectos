using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Background;

public class GreenishBackground : IBackground{

    public void Render(RenderContext ctx){
        ctx.Display(
            draw: dc => {
                //Base
                var color = Color.Parse("#80BC62");
                var brush = new SolidColorBrush(color);
                dc.FillRectangle(brush, new Rect(ctx.Bounds.Size));

                //Top section
                color = Color.Parse("#62BC9E");
                brush = new SolidColorBrush(color);
                var topHeight = ctx.Bounds.Height * 0.04;
                dc.FillRectangle(brush, new Rect(0, 0, ctx.Bounds.Width, topHeight));

                //Separator line
                brush = new SolidColorBrush(color);
                double lineX = ctx.Bounds.Width * 2/3;
                dc.DrawLine(new Pen(brush, 2), new Point(lineX, 0), new Point(lineX, ctx.Bounds.Height));

            }
        );

    }

}
