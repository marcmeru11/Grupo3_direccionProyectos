using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Transition
{
    public class DefaultTransition : ITransition
    {
        private double _opacity = 1;
        private int _phase = 0;          // control de fases
        private readonly double _speed;
        private readonly Color _color;
        private SolidColorBrush _brush;

        public DefaultTransition(double speed = 0.02, Color? color = null)
        {
            _speed = speed;
            _color = color ?? Colors.Black;
            _brush = new SolidColorBrush(_color, _opacity);
        }

        public event Action? OnTransitionComplete;

        public bool IsComplete => _phase == 2;

        public void Render(RenderContext ctx)
        {
            if (_phase == 2)
            {
                // notificar fin
                if (!_notifiedComplete)
                {
                    OnTransitionComplete?.Invoke();
                    _notifiedComplete = true;
                }
                return;
            }

            // actualizar opacidad
            _opacity += (_phase == 0 ? _speed : -_speed);

            if (_opacity >= 1.0)
            {
                _opacity = 1.0;
                _phase = 1; // fade out
            }
            else if (_opacity <= 0.0 && _phase == 1)
            {
                _opacity = 0.0;
                _phase = 2; // terminado
            }

            _brush.Opacity = _opacity;

            // dibujar
            ctx.Display(dc =>
            {
                dc.FillRectangle(_brush, new Rect(ctx.Bounds.Size));
            });
        }

        public void Reset()
        {
            _opacity = 0;
            _phase = 0;
            _brush.Opacity = _opacity;
            _notifiedComplete = false;
        }

        public void Start()
        {
            Reset();
        }

        public void Skip()
        {
            _opacity = 0;
            _phase = 2;
            _brush.Opacity = _opacity;
            OnTransitionComplete?.Invoke();
        }

        private bool _notifiedComplete = false;
    }
}