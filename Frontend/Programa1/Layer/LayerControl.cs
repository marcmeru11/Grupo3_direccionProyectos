using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Style;

namespace Programa1.Layer;
    
/**
 * Core renderer bridge. To clarify, the whole rendering schema is built using different layers (each of which serve as an abstraction call to the
 * actual rendering methods, which actually render the content). These layers do computee the general settings of where the rendering should be cast on,
 * while the renderers (such as DefaultBackground) should just care about rendering the component.
 */
public sealed class LayerControl : Avalonia.Controls.Control {
    
    private readonly Window _window;
    private readonly List<IRenderLayer> _layers;

    public LayerControl(Window window, List<IRenderLayer> layers) {
        _window = window;
        _layers = layers;
    }

    /// <summary>
    /// Renders the visual content divided into different layers. Each layer serves as a bridge
    /// to an abstraction that renders the corresponding style type based on those specified
    /// in <see cref="RenderStyle"/>.
    /// </summary>
    public override void Render(DrawingContext drawingContext) {
        
        // Just in case the context draws above the renderer layers
        base.Render(drawingContext);

        // TODO: Build RenderStyles from the ground up using the factory instead of hardcoding default styles
        foreach (IRenderLayer layer in _layers) {
            layer.Render(RenderContext.Of(_window).Begin(drawingContext), RenderStyle.Default());
        }
        
    }
    
    /**
     * Hotfix for window stutter when manually moving the main app window
     */
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);
        GfxFlushLoop();
    }

    private void GfxFlushLoop() {
        TopLevel? top = TopLevel.GetTopLevel(this);
        if (top is null) {
            return;
        }

        top.RequestAnimationFrame(_ => {
            InvalidateVisual();
            GfxFlushLoop();
        });
        
    }
    
}