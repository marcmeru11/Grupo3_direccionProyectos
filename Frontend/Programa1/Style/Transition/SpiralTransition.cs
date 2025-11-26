using System;
using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Transition
{
    public class SpiralTransition : ITransition
    {
        private double _progress = 1;
        private int _phase = 1;
        private readonly double _speed;
        private readonly bool _clockwise;
        private readonly SolidColorBrush _brush;
        private bool _notifiedComplete = false;

        public event Action? OnTransitionComplete;
        public bool IsComplete => _phase == 2;

        public SpiralTransition(double speed = 0.02, bool clockwise = true)
        {
            _speed = speed;
            _clockwise = clockwise;
            _brush = new SolidColorBrush(Colors.Black);
        }

        public void Render(RenderContext ctx)
        {
            if (_phase == 2)
            {
                if (!_notifiedComplete)
                {
                    OnTransitionComplete?.Invoke();
                    _notifiedComplete = true;
                }
                return;
            }

            // actualizar progreso
            if (_phase == 1) _progress -= _speed;

            if (_progress <= 0 && _phase == 1)
            {
                _progress = 0;
                _phase = 2;
            }

            double sweep = _progress * 360 * (_clockwise ? 1 : -1);

            ctx.Display(dc =>
            {
                var c = new Point(ctx.Bounds.Width / 2, ctx.Bounds.Height / 2);
                double r = Math.Sqrt(Math.Pow(ctx.Bounds.Width, 2) + Math.Pow(ctx.Bounds.Height, 2));

                var geom = new StreamGeometry();
                using (var g = geom.Open())
                {
                    g.BeginFigure(c, true);
                    for (double a = 0; a <= Math.Abs(sweep); a += 3)
                    {
                        double rad = Math.PI * (a * (_clockwise ? 1 : -1)) / 180.0;
                        double x = c.X + r * Math.Cos(rad);
                        double y = c.Y + r * Math.Sin(rad);
                        g.LineTo(new Point(x, y));
                    }
                    g.LineTo(c);
                    g.EndFigure(true);
                }

                dc.DrawGeometry(_brush, null, geom);
            });
        }

        public void Reset()
        {
            _progress = 1;
            _phase = 1;
            _notifiedComplete = false;
        }

        public void Start() => Reset();

        public void Skip()
        {
            _phase = 2;
            OnTransitionComplete?.Invoke();
        }
    }
}
