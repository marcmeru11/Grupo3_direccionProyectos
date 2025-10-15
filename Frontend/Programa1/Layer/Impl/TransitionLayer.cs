using Programa1.layer;
using Programa1.Style;

namespace Programa1.Layer.Impl;

public class TransitionLayer : IRenderLayer {
    
    public void Render(RenderContext context, RenderStyle style) {
        style.RenderTransition(context);
    }
    
}