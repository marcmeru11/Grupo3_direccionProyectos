using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Transition;

public class BlockFadeTransition : ITransition
{
    private const int Columns = 10;
    private const int Rows = 6;
    private readonly double[,] _opacities = new double[Columns, Rows];
    private double _progress = 0.0;
    private const double _speed = 0.01;

    public BlockFadeTransition()
    {
        for (int x = 0; x < Columns; x++)
            for (int y = 0; y < Rows; y++)
                _opacities[x, y] = 1.0;
    }

    public void Render(RenderContext ctx)
    {
        double width = ctx.Bounds.Width;
        double height = ctx.Bounds.Height;
        double blockWidth = width / Columns;
        double blockHeight = height / Rows;

        if (_progress < 1.0)
            _progress += _speed;

        int totalBlocks = Columns * Rows;

        int blocksToFade = (int)(_progress * totalBlocks);

        int count = 0;
        for (int x = Columns - 1; x >= 0; x--)
        {
            for (int y = 0; y < Rows; y++)
            {
                if (count < blocksToFade)
                {
                    _opacities[x, y] -= _speed * 1.75;
                    if (_opacities[x, y] < 0)
                        _opacities[x, y] = 0;
                }
                count++;
            }
        }

        ctx.Display(dc =>
        {
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    var brush = new SolidColorBrush(Colors.Black, _opacities[x, y]);
                    var rect = new Rect(x * blockWidth, y * blockHeight, blockWidth, blockHeight);
                    dc.FillRectangle(brush, rect);
                }
        });
    }
}
