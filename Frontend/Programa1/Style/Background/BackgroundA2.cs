using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Background;

public class BackgroundA2 : IBackground
{

    public void Render(RenderContext ctx)
    {
        ctx.Display(
            draw: dc =>
            {
                //Base
                var color = Color.Parse("#1C1C1C");
                var brush = new SolidColorBrush(color);
                dc.FillRectangle(brush, new Rect(ctx.Bounds.Size));

                //Top section
                color = Color.Parse("#212121");
                brush = new SolidColorBrush(color);
                var topHeight = ctx.Bounds.Height * 0.04;
                dc.FillRectangle(brush, new Rect(0, 0, ctx.Bounds.Width, topHeight));

                //Color for separator lines (Both X and Y)
                color = Color.Parse("#262626");
                brush = new SolidColorBrush(color);

                //Separator lineX
                double lineX = topHeight;
                dc.DrawLine(new Pen(brush, 2), new Point(0, topHeight), new Point(ctx.Bounds.Width, topHeight));

                //Separator lineY
                double posLineY = ctx.Bounds.Width * 2 / 3;
                dc.DrawLine(new Pen(brush, 2), new Point(posLineY, topHeight), new Point(posLineY, ctx.Bounds.Height));

                //Implementing image pattern in the background
                var image = new Bitmap("Assets/brillitos.png");
                dc.DrawImage(image, new Rect(0, 0, ctx.Bounds.Width, ctx.Bounds.Height));

            }

        );

    }

}
