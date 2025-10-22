using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Transition;

public class DefaultTransition : ITransition
{
    private double _opacity = 0;          
    private bool _fadeIn = true;          
    private readonly DispatcherTimer _timer;

    public DefaultTransition()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _timer.Tick += (_, _) =>
        {
            double speed = 0.05;

            if (_fadeIn)
            {
                _opacity += speed;
                if (_opacity >= 1)
                {
                    _opacity = 1;
                    _fadeIn = false; 
                }
            }
            else
            {
                _opacity -= speed;
                if (_opacity <= 0)
                {
                    _opacity = 0;
                    _fadeIn = true; 
                }
            }
        };
        _timer.Start();
    }

    public void Render(RenderContext ctx)
    {
        ctx.Display(draw: dc =>
        {
            dc.FillRectangle(Brushes.Blue, new Rect(ctx.Bounds.Size));

            var fadeBrush = new SolidColorBrush(Colors.Black, _opacity);
            dc.FillRectangle(fadeBrush, new Rect(ctx.Bounds.Size));
        });
    }
}
