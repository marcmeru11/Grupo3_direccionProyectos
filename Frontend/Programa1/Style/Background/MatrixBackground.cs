using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using HarfBuzzSharp;
using Programa1.layer;
using Programa1.Layer.Bridge;
using Programa1.Util;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Programa1.Style.Background;

public class MatrixBackground : IBackground{

    private readonly Timer timer = new Timer(30); // El argumento entre 60 (fps) son los segundos (30/60 = 0.5s)
    private static Random random = new Random();
    private List<char> letras = [];

    public void AñadirLetraAleatoria()
    {
        
        int index = random.Next(MatrixLetters.Lista.Length);
        char letra = MatrixLetters.Lista[index];
        letras.Add(letra);
    }

    public void Render(RenderContext ctx){
        ctx.Display(
            draw: dc => {
                //Base
                var color = Color.Parse("#0F0F0F");
                var brush = new SolidColorBrush(color);
                dc.FillRectangle(brush, new Rect(ctx.Bounds.Size));


                //Columnas de Matrix
                if (timer.Tick())
                {
                    AñadirLetraAleatoria();
                    int y = 0;
                    foreach (char letra in letras)
                    {
                        
                        FormattedText texto = new FormattedText(
                        letra.ToString(),
                        CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        32,
                        Brushes.Lime);

                        dc.DrawText(texto, new Point(0, y));
                        y += 100; // espacio entre líneas
                    }
                }
                
            }
        );

    }

}
