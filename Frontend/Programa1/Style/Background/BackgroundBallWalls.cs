using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;
using Programa1.Util;

namespace Programa1.Style.Background;

public class BackgroundBallWalls : IBackground
{
    private readonly Timer timer = new Timer(4); // El argumento entre 60 (fps) son los segundos (30/60 = 0.5s)
    private Point posicion = new Point(0, 0);
    private double posX = 0, posY = 0;
    private bool goingHor = false, goingVert = false; // Hor = false -> Izq | Vert = false -> Abajo

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
                    if (goingHor) posX += 0.25;
                    else posX -= 0.25;
                    if (goingVert) posY += 0.25;
                    else posY -= 0.25;

                    

                    posicion = new Point(posX, posY);

                }
                
                // Dibujar círculo
                dc.DrawEllipse(new SolidColorBrush(Color.Parse("#FFABEF")), null, posicion, 200, 200);
            }
        );
    }
}
