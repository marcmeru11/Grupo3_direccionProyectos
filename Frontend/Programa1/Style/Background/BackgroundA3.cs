using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Background;

public class BackgroundA3 : IBackground
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

                //Separator lineY
                double posLineY = ctx.Bounds.Width * 2 / 3;
                dc.DrawLine(new Pen(brush, 2), new Point(posLineY, topHeight), new Point(posLineY, ctx.Bounds.Height));

                //Background animation
                color = Color.Parse("#FDFDFC");
                brush = new SolidColorBrush(color);
                var rand = new Random();
                double posX = rand.NextDouble()*(ctx.Bounds.Width);
                double posY = rand.NextDouble()*(ctx.Bounds.Height);
                dc.DrawEllipse(brush, null, new Point(posX, posY), 5, 5);

            }

        );

    }

}