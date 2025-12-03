using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;
using Programa1.Util;

namespace Programa1.Style.Background;

public class BackgroundBall : IBackground
{
    private readonly Timer timer = new Timer(1); // El argumento entre 60 (fps) son los segundos (30/60 = 0.5s)
    private Point posicion = new Point(0, 0);
    private double posX = 0;
    private bool goingRight = true;

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

                // Actualizar posición cada 0.5 segundos
                if (timer.Tick())
                {
                    if (goingRight)
                    {
                        posX += 0.25;
                        if (posX >= ctx.Bounds.Width)
                        {
                            goingRight = false;
                        }
                    }
                    else
                    {
                        posX -= 0.25;
                        if (posX <= 0)
                        {
                            goingRight = true;
                        }
                    }
                    posicion = new Point(posX, ctx.Bounds.Height);
                }
                
                // Dibujar círculo
                dc.DrawEllipse(new SolidColorBrush(Color.Parse("#FFABEF")), null, posicion, 40, 40);
            }
        );
    }
}
