using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using System;
using System.Linq;

namespace Programa1.Style.Transition
{
    public class MosaicTransition : ITransition
    {
        private double _progress = 0;
        private int _phase = 0; // fases
        private readonly double _speed;
        private readonly Color _color;
        private readonly int _tilesX;
        private readonly int _tilesY;
        private readonly MosaicPattern _pattern;
        private readonly SolidColorBrush _brush;
        private bool _notifiedComplete = false;
        private readonly Random _random = new Random();
        private readonly int[] _tileOrder;

        public MosaicTransition(double speed = 0.015, Color? color = null, int tilesX = 21, int tilesY = 21, MosaicPattern pattern = MosaicPattern.Spiral)
        {
            _speed = speed;
            _color = color ?? Colors.Black;
            _tilesX = tilesX;
            _tilesY = tilesY;
            _pattern = pattern;
            _brush = new SolidColorBrush(_color);
            _tileOrder = GenerateTileOrder();
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

            // dibujar mosaico
            ctx.Display(dc =>
            {
                double width = ctx.Bounds.Width;
                double height = ctx.Bounds.Height;
                double tileWidth = width / _tilesX;
                double tileHeight = height / _tilesY;

                int totalTiles = _tilesX * _tilesY;
                int tilesToProcess = (int)(_progress * totalTiles);

                for (int i = 0; i < totalTiles; i++)
                {
                    int tileIndex = _tileOrder[i];
                    int x = tileIndex % _tilesX;
                    int y = tileIndex / _tilesX;

                    double tileProgress;

                    if (_phase == 0)
                    {
                        // aparecer: tile activo si está en procesamiento
                        tileProgress = i < tilesToProcess ? 1.0 : 0.0;
                    }
                    else
                    {
                        // desaparecer: tile activo si NO está en procesamiento
                        tileProgress = i >= tilesToProcess ? 1.0 : 0.0;
                    }

                    if (tileProgress > 0)
                    {
                        DrawTile(dc, x, y, tileWidth, tileHeight, tileProgress);
                    }
                }
            });

            // cambiar fase cuando todos los tiles estén procesados
            if (_progress >= 1.0)
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

        private void DrawTile(DrawingContext dc, int x, int y, double tileWidth, double tileHeight, double opacity)
        {
            double posX = x * tileWidth;
            double posY = y * tileHeight;

            var brush = new SolidColorBrush(_color, opacity);
            dc.FillRectangle(brush, new Rect(posX, posY, tileWidth, tileHeight));
        }

        private int[] GenerateTileOrder()
        {
            int totalTiles = _tilesX * _tilesY;
            var tiles = new (int index, int position)[totalTiles];

            for (int i = 0; i < totalTiles; i++)
            {
                int x = i % _tilesX;
                int y = i / _tilesX;

                tiles[i] = (i, CalculateTilePosition(x, y));
            }

            // ordenar por posición
            return tiles.OrderBy(t => t.position).Select(t => t.index).ToArray();
        }

        private int CalculateTilePosition(int x, int y)
        {
            return _pattern switch
            {
                MosaicPattern.Spiral => CalculateSpiralPosition(x, y),
                MosaicPattern.CenterOut => CalculateCenterPosition(x, y),
                MosaicPattern.Random => _random.Next(1000),
                MosaicPattern.Rows => y * _tilesX + x,
                MosaicPattern.Columns => x * _tilesY + y,
                _ => 0
            };
        }

        private int CalculateSpiralPosition(int x, int y)
        {
            int centerX = _tilesX / 2;
            int centerY = _tilesY / 2;

            // distancia desde el centro (espiral)
            int dx = Math.Abs(x - centerX);
            int dy = Math.Abs(y - centerY);
            return dx * dx + dy * dy;
        }

        private int CalculateCenterPosition(int x, int y)
        {
            int centerX = _tilesX / 2;
            int centerY = _tilesY / 2;

            // distancia Manhattan desde el centro
            return Math.Abs(x - centerX) + Math.Abs(y - centerY);
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

    public enum MosaicPattern
    {
        Spiral,     // espiral desde el centro
        CenterOut,  // anillos concéntricos
        Random,     // orden aleatorio
        Rows,       // filas de arriba a abajo
        Columns     // columnas de izquierda a derecha
    }
}