using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Programa1.layer;
using Programa1.Layer.Bridge;
using Programa1.Util;
using System;

namespace Programa1.Style.Background;

public class BackgroundBallGradiente : IBackground
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
                var color = Color.Parse("#FFB6C1");
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
                        posX += 0.10;
                        if (posX >= ctx.Bounds.Width)
                        {
                            goingRight = false;
                        }
                    }
                    else
                    {
                        posX -= 0.10;
                        if (posX <= 0)
                        {
                            goingRight = true;
                        }
                    }
                    
                }

                var gradientBrush = new RadialGradientBrush
                {
                    Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative), // Centro del gradiente
                    GradientOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative), // Punto desde donde comienza
                    RadiusX = new RelativeScalar(0.5, RelativeUnit.Relative), // Radio relativo (0.5 = ocupa todo el círculo)
                    RadiusY = new RelativeScalar(0.5, RelativeUnit.Relative),
                    SpreadMethod = GradientSpreadMethod.Pad,
                    GradientStops = new GradientStops
                    {
                        new GradientStop(Color.Parse("#e2ddd0"), 0), // Centro
                        new GradientStop(Color.Parse("#FFB6C1"), 1)  // Borde (Mismo color que el fondo)
                    }
                };

                // Dibujar el círculo
                dc.DrawEllipse(gradientBrush, null, posicion, 480, 480);

                if (Math.Abs(posX - ctx.Bounds.Width / 2.0) < 0.5)
                {
                    var image = new Bitmap("Assets/susto.png");
                    dc.DrawImage(image, new Rect(0, 0, ctx.Bounds.Width, ctx.Bounds.Height));
                }
                posicion = new Point(posX, ctx.Bounds.Height + 160);
            }
        );
    }
}
