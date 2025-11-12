using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Transition
{
    public class BlindsTransition : ITransition
    {
        private double _progress = 0;
        private int _phase = 0; // fases
        private readonly double _speed;
        private readonly Color _color;
        private readonly int _blindsCount;
        private readonly BlindsDirection _direction;
        private readonly SolidColorBrush _brush;
        private bool _notifiedComplete = false;

        public BlindsTransition(double speed = 0.02, Color? color = null, int blindsCount = 8, BlindsDirection direction = BlindsDirection.Horizontal)
        {
            _speed = speed;
            _color = color ?? Colors.Black;
            _blindsCount = blindsCount;
            _direction = direction;
            _brush = new SolidColorBrush(_color);
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

            // actualizar progreso
            _progress += _speed;

            // dibujar persianas
            ctx.Display(dc =>
            {
                double width = ctx.Bounds.Width;
                double height = ctx.Bounds.Height;

                if (_direction == BlindsDirection.Horizontal)
                {
                    // persianas horizontales
                    double blindHeight = height / _blindsCount;
                    for (int i = 0; i < _blindsCount; i++)
                    {
                        double blindProgress = CalculateBlindProgress(i);
                        double currentHeight = blindHeight * blindProgress;
                        double y = i * blindHeight;

                        dc.FillRectangle(_brush, new Rect(0, y, width, currentHeight));
                    }
                }
                else
                {
                    // persianas verticales
                    double blindWidth = width / _blindsCount;
                    for (int i = 0; i < _blindsCount; i++)
                    {
                        double blindProgress = CalculateBlindProgress(i);
                        double currentWidth = blindWidth * blindProgress;
                        double x = i * blindWidth;

                        dc.FillRectangle(_brush, new Rect(x, 0, currentWidth, height));
                    }
                }
            });

            // cambiar fase cuando el último llegue al final
            if (_progress >= 1.0 + ((_blindsCount - 1) * 0.1))
            {
                if (_phase == 0)
                {
                    _phase = 1;
                    _progress = 0;
                }
                else
                {
                    _phase = 2;
                }
            }
        }

        private double CalculateBlindProgress(int blindIndex)
        {
            if (_phase == 0)
            {
                // abrir: progreso escalonado
                double blindStart = blindIndex * 0.1;
                return Math.Max(0, Math.Min(1, (_progress - blindStart) / (1 - blindStart)));
            }
            else
            {
                // cerrar: progreso escalonado inverso
                double blindStart = (_blindsCount - 1 - blindIndex) * 0.1;
                return Math.Max(0, Math.Min(1, (1 - (_progress - blindStart) / (1 - blindStart))));
            }
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

    public enum BlindsDirection
    {
        Horizontal, // persianas horizontales
        Vertical    // persianas verticales
    }
}