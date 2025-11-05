using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Transition
{
    public class ShapeTransition : ITransition
    {
        private double _progress = 0;
        private int _phase = 0; // fases
        private readonly double _speed;
        private readonly Color _color;
        private SolidColorBrush _brush;
        private bool _notifiedComplete = false;

        public ShapeTransition(double speed = 0.02, Color? color = null)
        {
            _speed = speed;
            _color = color ?? Colors.Black;
            _brush = new SolidColorBrush(_color);
        }

        public event Action? OnTransitionComplete;

        public bool IsComplete => _phase == 2;

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
            _progress += (_phase == 0 ? _speed : -_speed);

            if (_progress >= 1.0)
            {
                _progress = 1.0;
                _phase = 1; // contracción
            }
            else if (_progress <= 0.0 && _phase == 1)
            {
                _progress = 0.0;
                _phase = 2; // terminado
            }

            // dibujar círculo
            ctx.Display(dc =>
            {
                var center = new Point(ctx.Bounds.Width / 2, ctx.Bounds.Height / 2);
                double maxSize = Math.Max(ctx.Bounds.Width, ctx.Bounds.Height);
                double size = maxSize * _progress * 1.5;

                var geometry = new StreamGeometry();
                using (var gctx = geometry.Open())
                {
                    gctx.BeginFigure(new Point(center.X + size / 2, center.Y), true);
                    gctx.ArcTo(new Point(center.X - size / 2, center.Y),
                               new Size(size / 2, size / 2),
                               360, true, SweepDirection.Clockwise);
                    gctx.ArcTo(new Point(center.X + size / 2, center.Y),
                               new Size(size / 2, size / 2),
                               360, true, SweepDirection.Clockwise);
                    gctx.EndFigure(true);
                }

                dc.DrawGeometry(_brush, null, geometry);
            });
        }

        public void Reset()
        {
            _progress = 0;
            _phase = 0;
            _notifiedComplete = false;
        }

        public void Start()
        {
            Reset();
        }

        public void Skip()
        {
            _progress = 0;
            _phase = 2;
            OnTransitionComplete?.Invoke();
        }
    }
}
