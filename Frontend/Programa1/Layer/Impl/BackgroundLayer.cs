using Programa1.layer;
using Programa1.Style;

namespace Programa1.Layer.Impl;

public class BackgroundLayer : IRenderLayer {
    
    public void Render(RenderContext context, RenderStyle style) {
        style.RenderBackground(context);
    }
    
}