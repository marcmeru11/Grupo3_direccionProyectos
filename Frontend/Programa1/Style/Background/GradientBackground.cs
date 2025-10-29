using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Background;

public class GradientBackground : IBackground{

    public void Render(RenderContext ctx){
        ctx.Display(
            draw: dc => {
                //Base gradiente
                var gradientBrush = new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative), // Horizontal (Para cambiar a vertical, usar (0,1))
                    GradientStops = new GradientStops // Colores del gradiente
                    {
                        new GradientStop(Color.Parse("#CCD5F0"), 0.0), // Claro
                        new GradientStop(Color.Parse("#5D6B99"), 0.5)  // Oscuro
                    }
                };

                dc.FillRectangle(gradientBrush, new Rect(ctx.Bounds.Size));

                //Top section
                var color = Color.Parse("#CCD5F0");
                var brush = new SolidColorBrush(color);
                var topHeight = ctx.Bounds.Height * 0.04;
                dc.FillRectangle(brush, new Rect(0, 0, ctx.Bounds.Width, topHeight));

                //Separator line
                color = Color.Parse("#CCD5F0");
                brush = new SolidColorBrush(color);
                double lineX = ctx.Bounds.Width * 2/3;
                dc.DrawLine(new Pen(brush, 2), new Point(lineX, 0), new Point(lineX, ctx.Bounds.Height));

                //Second "base"
                color = Color.Parse("#3E0070");
                brush = new SolidColorBrush(color);
                dc.FillRectangle(brush, new Rect(2/3, 0, ctx.Bounds.Width, topHeight));

            }
        );

    }

}
