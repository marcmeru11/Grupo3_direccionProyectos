using Avalonia;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;

namespace Programa1.Style.Background;

public class DefaultBackground : IBackground {
    
    public void Render(RenderContext ctx) {
        ctx.Display(
            draw: dc => {
               dc.FillRectangle(Brushes.White, new Rect(ctx.Bounds.Size));
                dc.FillRectangle(Brushes.White, new Rect(ctx.Bounds.Size));
            }
        );

    }
    
}