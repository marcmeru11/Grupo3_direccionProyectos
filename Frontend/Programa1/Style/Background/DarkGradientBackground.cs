using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;
using Programa1.Util;

namespace Programa1.Style.Background;

public class DarkGradientBackground : IBackground
{
    private readonly Timer timer = new Timer(1); // El argumento entre 60 (fps) son los segundos (30/60 = 0.5s)
    private Point position = new Point(0, 0);
    private double radius = 0;

    public void Render(RenderContext ctx)
    {
        ctx.Display(
            draw: dc => {
                //Base
                var gradientBrush = new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative), // Horizontal (Para cambiar a vertical, usar (0,1))
                    GradientStops = new GradientStops // Colores del gradiente
                    {
                        new GradientStop(Color.Parse("#3A3A3A"), 0.0), // Claro
                        new GradientStop(Color.Parse("#191919"), 0.66)  // Oscuro
                    }
                };

                dc.FillRectangle(gradientBrush, new Rect(ctx.Bounds.Size));

                var color = Color.Parse("#CCD5F0");
                var brush = new SolidColorBrush(color);
                var topHeight = ctx.Bounds.Height * 0.04;
                dc.FillRectangle(brush, new Rect(0, 0, ctx.Bounds.Width, topHeight));

                color = Color.Parse("#191919");
                brush = new SolidColorBrush(color);
                dc.FillRectangle(brush, new Rect(1 / 3, 0, ctx.Bounds.Width, topHeight));

                // Actualizar posición cada 0.5 segundos
                if (timer.Tick())
                {
                    radius += 0.25;
                    position = new Point(radius, ctx.Bounds.Height);
                }

                var center = new Point(ctx.Bounds.Width / 2, ctx.Bounds.Height / 2);
                var blackBrush = new SolidColorBrush(Color.Parse("#191919"));

                dc.DrawEllipse(blackBrush, null, center, radius, radius);

            }
        );

    }

}
