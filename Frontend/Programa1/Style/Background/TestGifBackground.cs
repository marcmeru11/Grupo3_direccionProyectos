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

public class TestGifBackGround : IBackground
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



/*
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Programa1.Style.Background
{
    public class GifBackground : IBackground
    {
        private List<Bitmap> _frames;
        private int _currentFrame = 0;
        private DispatcherTimer _timer;

        public GifBackground(string gifPath)
        {
            LoadGifFrames(gifPath);

            // Timer para avanzar frames
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100); // Ajusta según tu GIF
            _timer.Tick += (s, e) =>
            {
                _currentFrame = (_currentFrame + 1) % _frames.Count;
            };
            _timer.Start();
        }

        private void LoadGifFrames(string gifPath)
        {
            _frames = new List<Bitmap>();

            using (var stream = File.OpenRead(gifPath))
            using (var img = System.Drawing.Image.FromStream(stream))
            {
                var dimension = new System.Drawing.Imaging.FrameDimension(img.FrameDimensionsList[0]);
                int frameCount = img.GetFrameCount(dimension);

                for (int i = 0; i < frameCount; i++)
                {
                    img.SelectActiveFrame(dimension, i);

                    using (var ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        _frames.Add(new Bitmap(ms));
                    }
                }
            }
        }

        public void Render(RenderContext ctx)
        {
            ctx.Display(draw: dc =>
            {
                // Dibujar fondo base (como tu BeigeBackground)
                var baseColor = Color.Parse("#CFB997");
                var brush = new SolidColorBrush(baseColor);
                dc.FillRectangle(brush, new Rect(ctx.Bounds.Size));

                // Dibujar GIF encima, centrado y escalado
                if (_frames.Count > 0)
                {
                    var frame = _frames[_currentFrame];
                    double scaleX = ctx.Bounds.Width / frame.Size.Width;
                    double scaleY = ctx.Bounds.Height / frame.Size.Height;
                    double scale = Math.Min(scaleX, scaleY); // Mantener proporción

                    double width = frame.Size.Width * scale;
                    double height = frame.Size.Height * scale;

                    double offsetX = (ctx.Bounds.Width - width) / 2;
                    double offsetY = (ctx.Bounds.Height - height) / 2;

                    dc.DrawImage(frame, 1, new Rect(0, 0, frame.Size.Width, frame.Size.Height),
                        new Rect(offsetX, offsetY, width, height));
                }

                // Top section y línea de separación (igual que tu BeigeBackground)
                var separatorColor = Color.Parse("#AC9362");
                brush = new SolidColorBrush(separatorColor);
                var topHeight = ctx.Bounds.Height * 0.04;
                dc.FillRectangle(brush, new Rect(0, 0, ctx.Bounds.Width, topHeight));

                double lineX = ctx.Bounds.Width * 2 / 3;
                dc.DrawLine(new Pen(brush, 2), new Point(lineX, 0), new Point(lineX, ctx.Bounds.Height));
            });
        }
    }
}


 
*/
