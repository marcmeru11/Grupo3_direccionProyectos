using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;

namespace Programa1.Style.Transition
{
    public class BlockFadeTransition : ITransition
    {
        private const int DefaultColumns = 10;
        private const int DefaultRows = 6;
        private const double DefaultSpeed = 0.015;

        private readonly int _columns;
        private readonly int _rows;
        private readonly double _speed;
        private readonly BlockDirection _direction;
        private readonly double[,] _opacities;

        private double _progress = 0.0;
        private int _phase = 0;
        private bool _notifiedComplete = false;
        private bool _allBlocksFaded = false;

        public BlockFadeTransition(double speed = DefaultSpeed,
                                 int columns = DefaultColumns,
                                 int rows = DefaultRows,
                                 BlockDirection direction = BlockDirection.RightToLeft) // direccion
        {
            _speed = speed;
            _columns = columns;
            _rows = rows;
            _direction = direction;
            _opacities = new double[_columns, _rows];
            ResetOpacities();
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

            _progress += _speed;

            double width = ctx.Bounds.Width;
            double height = ctx.Bounds.Height;
            double blockWidth = width / _columns;
            double blockHeight = height / _rows;

            int totalBlocks = _columns * _rows;
            int blocksToFade = Math.Min(totalBlocks, (int)(_progress * totalBlocks * 2)); //
            UpdateOpacities(blocksToFade);

            // verificar si todos los bloques están transparentes
            CheckAllBlocksFaded();

            ctx.Display(dc =>
            {
                for (int x = 0; x < _columns; x++)
                    for (int y = 0; y < _rows; y++)
                    {
                        var brush = new SolidColorBrush(Colors.Black, _opacities[x, y]);
                        var rect = new Rect(x * blockWidth, y * blockHeight, blockWidth, blockHeight);
                        dc.FillRectangle(brush, rect);
                    }
            });

            // esperar hasta que todos los bloques estén transparentes
            if (_allBlocksFaded)
            {
                _phase = 2;
            }
        }

        private void UpdateOpacities(int blocksToFade)
        {
            int count = 0;

            switch (_direction)
            {
                case BlockDirection.RightToLeft:
                    for (int x = _columns - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < _rows; y++)
                        {
                            if (count < blocksToFade)
                            {
                                _opacities[x, y] -= _speed * 1.75;
                                if (_opacities[x, y] < 0) _opacities[x, y] = 0;
                            }
                            count++;
                        }
                    }
                    break;

                case BlockDirection.LeftToRight:
                    for (int x = 0; x < _columns; x++)
                    {
                        for (int y = 0; y < _rows; y++)
                        {
                            if (count < blocksToFade)
                            {
                                _opacities[x, y] -= _speed * 1.75;
                                if (_opacities[x, y] < 0) _opacities[x, y] = 0;
                            }
                            count++;
                        }
                    }
                    break;

                case BlockDirection.TopToBottom:
                    for (int y = 0; y < _rows; y++)
                    {
                        for (int x = 0; x < _columns; x++)
                        {
                            if (count < blocksToFade)
                            {
                                _opacities[x, y] -= _speed * 1.75;
                                if (_opacities[x, y] < 0) _opacities[x, y] = 0;
                            }
                            count++;
                        }
                    }
                    break;

                case BlockDirection.BottomToTop:
                    for (int y = _rows - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < _columns; x++)
                        {
                            if (count < blocksToFade)
                            {
                                _opacities[x, y] -= _speed * 1.75;
                                if (_opacities[x, y] < 0) _opacities[x, y] = 0;
                            }
                            count++;
                        }
                    }
                    break;
            }
        }

        private void CheckAllBlocksFaded()
        {
            _allBlocksFaded = true;
            for (int x = 0; x < _columns; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    if (_opacities[x, y] > 0.01)
                    {
                        _allBlocksFaded = false;
                        return;
                    }
                }
            }
        }

        private void ResetOpacities()
        {
            for (int x = 0; x < _columns; x++)
                for (int y = 0; y < _rows; y++)
                    _opacities[x, y] = 1.0;

            _allBlocksFaded = false;
        }

        public void Reset()
        {
            _progress = 0;
            _phase = 0;
            ResetOpacities();
            _notifiedComplete = false;
        }

        public void Start() => Reset();

        public void Skip()
        {
            for (int x = 0; x < _columns; x++)
                for (int y = 0; y < _rows; y++)
                    _opacities[x, y] = 0.0;

            _phase = 2;
            OnTransitionComplete?.Invoke();
        }
    }

    public enum BlockDirection
    {
        RightToLeft,    // derecha a izquierda (original)
        LeftToRight,    // izquierda a derecha
        TopToBottom,    // arriba a abajo
        BottomToTop     // abajo a arriba
    }
}