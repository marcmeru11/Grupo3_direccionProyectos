using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Transition
{
    public class DefaultTransition : ITransition
    {
        private double _opacity = 0;       
        private int _phase = 0;          
        private readonly double _speed;
        private readonly SolidColorBrush _brush;

        public DefaultTransition(double speed = 0.02)
        {
            _speed = speed;
            _brush = new SolidColorBrush(Colors.Black, _opacity);
        }

        public void Render(RenderContext ctx)
        {
            if (_phase == 2)
                return;

            _opacity += (_phase == 0 ? _speed : -_speed);

            if (_opacity >= 1)
            {
                _opacity = 1;
                _phase = 1; 
            }
            else if (_opacity <= 0 && _phase == 1)
            {
                _opacity = 0;
                _phase = 2; 
            }

            _brush.Opacity = _opacity;

            ctx.Display(dc =>
            {
                dc.FillRectangle(_brush, new Rect(ctx.Bounds.Size));
            });
        }
    }
}
