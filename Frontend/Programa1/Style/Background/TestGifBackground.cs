using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Programa1.layer;
using Programa1.Layer.Bridge;
/// using AvaloniaGif;

namespace Programa1.Style.Background;

/// <summary>
/// 
/// THIS FILE IS FOR TESTING PURPOSES.
/// IT IS A COPY OF "BackgroundA2" BUT
/// USES A GIF INSTEAD OF A STATIC IMAGE.
/// 
/// To accomplish this, the "AvaloniaGif" NuGet package
/// must be installed in the project.
/// 
/// "AvaloniaGif-Fork" is the avaiable package. 
/// Version 1.0.1.
/// 
/// WARNING: This implementation may have major structural
/// issues, as it might require to change the rendering
/// structure of the application for it to work properly.
/// 
/// </summary>

public class testGifBackGround : IBackground
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

                //Implementing gif in the background

                /* 
                var gif = new GifImage
                {
                    Source = new Avalonia.Media.Imaging.Bitmap("Assets/animacion.gif"),
                    Stretch = Avalonia.Media.Stretch.Uniform
                };
                */

            }

        );

    }

}
