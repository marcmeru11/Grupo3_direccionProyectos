using System.Collections.Generic;
using Avalonia.Controls;
using Programa1.Layer;
using Programa1.Layer.Impl;

namespace Programa1;

public partial class Programa1 : Window {

    private static readonly List<IRenderLayer> Layers = new();

    /**
     * Just meant for static initialization of the final list above
     */
    static Programa1()
    {
        Layers.Add(new BackgroundLayer());
        Layers.Add(new UploadLayer());
        Layers.Add(new StyleSelectorLayer());
        Layers.Add(new TransitionLayer());
    }

    /**
     * Relevant main window params and grid hook of the per-layer renderer
     */
    public Programa1() {
        Width = 500;
        Height = 500;
        Title = "Programa1";
        
        Grid root = new Grid();

        // Hook to the root grid for both overlay parsing and layer control
        root.Children.Add(new LayerControl(this, Layers));
        root.Children.Add(new Grid { Name = "OverlayRoot" });

        Content = root;
    }
    
}