using Programa1.layer;
using Programa1.Style;

namespace Programa1.Layer.Impl;
    
public class TransitionLayer : IRenderLayer {
    
    public void Render(RenderContext ctx, RenderStyle style) {
        style.RenderTransition(ctx);
    }
}
