using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Transition;

public class DefaultTransition : ITransition
{
    private double _opacity = 1;
    private bool _fadeIn = false;

    public void Render(RenderContext ctx)
    {
        const double speed = 0.02;
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

        ctx.Display(dc =>
        {
            var brush = new SolidColorBrush(Colors.Black, _opacity);
            dc.FillRectangle(brush, new Rect(ctx.Bounds.Size));
        });
    }
}
