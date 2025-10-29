using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Transition
{
    public class Slide2Transition : ITransition
    {
        private double _progress = 0;       
        private int _phase = 0;            
        private readonly double _speed;

        public Slide2Transition(double speed = 0.02)
        {
            _speed = speed;
        }

        public void Render(RenderContext ctx)
        {
            if (_phase == 2)
                return;

            _progress += _speed;
            if (_progress > 1) _progress = 1;

            double offsetX = 0;

            switch (_phase)
            {
                case 0: 
                    offsetX = ctx.Bounds.Width * (1 - _progress);
                    break;
                case 1: 
                    offsetX = -ctx.Bounds.Width * _progress;
                    break;
            }

            ctx.Display(dc =>
            {
                var brush = new SolidColorBrush(Colors.Black);
                dc.FillRectangle(brush, new Rect(offsetX, 0, ctx.Bounds.Width, ctx.Bounds.Height));
            });

            if (_progress >= 1)
            {
                if (_phase == 0)
                {
                    _phase = 1;
                    _progress = 0;
                }
                else if (_phase == 1)
                {
                    _phase = 2;
                }
            }
        }
    }
}
