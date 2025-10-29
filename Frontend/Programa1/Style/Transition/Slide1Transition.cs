using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Transition;

public class Slide1Transition : ITransition
{
    private double _offset = 0;
    private bool _slideIn = true;

    public void Render(RenderContext ctx)
    {
        const double speed = 10.0;
        double width = ctx.Bounds.Width;
        double height = ctx.Bounds.Height;

        if (_slideIn)
        {
            _offset += speed;
            if (_offset >= width)
            {
                _offset = width;
                _slideIn = false;
            }
        }
        else
        {
            _offset -= speed;
            if (_offset <= 0)
            {
                _offset = 0;
            }
        }

        ctx.Display(dc =>
        {
            var brush = new SolidColorBrush(Colors.Black);

            using (dc.PushTransform(Matrix.CreateTranslation(_offset - width, 0)))
            {
                var rect = new Rect(0, 0, width, height);
                dc.FillRectangle(brush, rect);
            }
        });
    }
}
