using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;
using Programa1.Util;

namespace Programa1.Style.Background;

public class BackgroundTimer : IBackground
{
    private readonly Timer timer = new Timer(120); // El argumento entre 60 (fps) son los segundos (30/60 = 0.5s)
    private readonly Random rand = new Random();
    private Point posicion = new Point(0, 0);

    public void Render(RenderContext ctx)
    {
        ctx.Display(
            draw: dc =>
            {
                // Fondo base
                var color = Color.Parse("#1C1C1C");
                var brush = new SolidColorBrush(color);
                dc.FillRectangle(brush, new Rect(ctx.Bounds.Size));

                // Sección superior
                color = Color.Parse("#212121");
                brush = new SolidColorBrush(color);
                var topHeight = ctx.Bounds.Height * 0.04;
                dc.FillRectangle(brush, new Rect(0, 0, ctx.Bounds.Width, topHeight));

                // Línea separadora
                color = Color.Parse("#262626");
                brush = new SolidColorBrush(color);
                double posLineY = ctx.Bounds.Width * 2 / 3;
                dc.DrawLine(new Pen(brush, 2), new Point(posLineY, topHeight), new Point(posLineY, ctx.Bounds.Height));

                // Actualizar posición cada 2 segundos
                if (timer.Tick())
                {
                    double x = rand.NextDouble() * ctx.Bounds.Width;
                    double y = rand.NextDouble() * ctx.Bounds.Height;
                    posicion = new Point(x, y);
                }

                // Dibujar círculo
                dc.DrawEllipse(new SolidColorBrush(Color.Parse("#FFABEF")), null, posicion, 5, 5);
            }
        );
    }
}
