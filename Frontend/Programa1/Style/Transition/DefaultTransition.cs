using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Transition;

public class DefaultTransition : ITransition
{
    public void Render(RenderContext ctx)
    {
        ctx.Display(draw: dc =>
        {

            // Tamaño del rectángulo
            double width = 100;
            double height = 100;

            // Posición centrada
            double x = (ctx.Bounds.Width - width) / 2;
            double y = (ctx.Bounds.Height - height) / 2;

            // Dibujar rectángulo azul
            dc.FillRectangle(Brushes.Blue, new Rect(x, y, width, height));
        });
    }
}
