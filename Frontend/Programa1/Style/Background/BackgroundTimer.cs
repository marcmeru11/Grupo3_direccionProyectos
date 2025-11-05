using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;
using Programa1.Util;

namespace Programa1.Style.Background;

public class BackgroundTimer : IBackground
{
    private bool mostrarRectangulo = false;

    // Método principal de renderizado
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

                // Rectángulo activado por el botón
                if (mostrarRectangulo)
                {
                    var rectBrush = new SolidColorBrush(Color.Parse("#F5CF27"));
                    var rect = new Rect(100, 100, 150, 80);
                    dc.DrawRectangle(rectBrush, null, rect);
                }
            },
            overlay: panel =>
            {
                var button = new Button
                {
                    Content = "Haz clic",
                    Width = 100,
                    Height = 40,
                    Margin = new Thickness(10)
                };

                button.Click += (_, _) =>
                {
                    Class1.botton1 = true;
                };

                panel.Children.Add(button);
            }
        );
    }
}
