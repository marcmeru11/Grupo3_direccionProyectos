using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Transition
{
    public class SlideTransition : ITransition
    {
        private double _progress = 0;
        private int _phase = 0;
        private readonly double _speed;
        private readonly SlideDirection _direction;
        private readonly SlideReturn _returnDirection;

        public SlideTransition(double speed = 0.02,
                             SlideDirection direction = SlideDirection.Left, // direccion de entrada
                             SlideReturn returnDirection = SlideReturn.Same) // direccion de regreso
        {
            _speed = speed;
            _direction = direction;
            _returnDirection = returnDirection;
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

            ctx.Display(dc =>
            {
                var brush = new SolidColorBrush(Colors.Black);
                double width = ctx.Bounds.Width;
                double height = ctx.Bounds.Height;

                SlideDirection currentDir = _phase == 0 ? _direction : GetReturnDirection();

                double offset = _phase == 0 ?
                    width * (1 - _progress) :  // entra
                    -width * _progress;        // sale

                if (currentDir == SlideDirection.Left)
                    offset = -offset;

                dc.FillRectangle(brush, new Rect(offset, 0, width, height));
            });

            // cambiar fase
            if (_progress >= 1)
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

        private SlideDirection GetReturnDirection()
        {
            return _returnDirection switch
            {
                SlideReturn.Same => _direction, 
                SlideReturn.Opposite => _direction == SlideDirection.Right ? SlideDirection.Left : SlideDirection.Right, // dirección opuesta
                _ => _direction
            };
        }

        public void Reset()
        {
            _progress = 0;
            _phase = 0;
            _notifiedComplete = false;
        }

        public void Start() => Reset();

        public void Skip()
        {
            _phase = 2;
            OnTransitionComplete?.Invoke();
        }

        private bool _notifiedComplete = false;
    }

    public enum SlideDirection
    {
        Left,
        Right
    }

    public enum SlideReturn
    {
        Same,      
        Opposite   
    }
}