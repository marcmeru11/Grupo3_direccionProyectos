using Programa1.layer;
using Programa1.Layer.Bridge;
using Programa1.Style.Background;

namespace Programa1.Style;

/**
 * Main renderer bridge that handles abstraction calls from most of the layers to their respective renderers
 */
public class RenderStyle {

    private readonly IBackground _background;
    
    public RenderStyle(IBackground background) {
        _background = background;
    }

    public static RenderStyle Default() {
        return new RenderStyle(new GradientBackground());
    }

    public void RenderBackground(RenderContext ctx) {
        _background.Render(ctx);
    }

    /// TODO: Keep creating different bridge methods. Do NOT use existing bridges if the renderer doesn't match the layer type (for example, don't call
    /// TODO: RenderBackground using RenderProgramLogo)
    /// <code>
    ///     public void RenderProgramLogo(RenderContext ctx) {
    ///         _programLogo.Render(ctx);
    ///     }
    /// </code>
    
}