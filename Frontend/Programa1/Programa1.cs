using System.Collections.Generic;
using Avalonia.Controls;
using Programa1.Layer;
using Programa1.Layer.Impl;

namespace Programa1;

public partial class Programa1 : Window {

    private static readonly List<IRenderLayer> LAYERS = new();

    static Programa1() {
        LAYERS.Add(new BackgroundLayer());
    }

    public Programa1() {
        Width = 500;
        Height = 500;
        Title = "Programa1";
    }
    
}