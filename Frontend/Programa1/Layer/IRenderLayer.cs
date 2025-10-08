using Programa1.layer;
using Programa1.Style;

namespace Programa1.Layer;

public interface IRenderLayer {
    void Render(RenderContext context, RenderStyle style);
}