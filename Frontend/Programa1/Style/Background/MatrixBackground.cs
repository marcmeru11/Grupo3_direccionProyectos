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
using System.Diagnostics;
using System.Globalization;

namespace Programa1.Style.Background;

public class MatrixBackground : IBackground{

    private readonly Timer timer = new Timer(1); // El argumento entre 60 (fps) son los segundos (30/60 = 0.5s)
    private static Random random = new Random();
    private List<char> lista1 = [], lista2 = [], lista3 = [], lista4 = [];
    private int c1 = 0, c2 = 0, c3 = 0, c4 = 0;
    private SolidColorBrush letterBrush = new SolidColorBrush(Color.Parse("#2dc724"));

    public void AñadirLetraAleatoria(List<char> listaActual, ref int contador)
    {
        if ( contador == 15)
        {
            // CON ESTO NO DECRECE, SOLO AUMENTA HASTA UN LIMITE.
            // Para que disminiya, habría que borrar las letras que sobrepasen el límite.
            for (int a = 0; a < random.Next(3, 11); a++)
            {
                int index = random.Next(MatrixLetters.Lista.Length);
                char letra = MatrixLetters.Lista[index];
                if (listaActual.Count > a)
                {
                    listaActual[a] = letra;
                }
                else
                {
                    listaActual.Add(letra);
                }
            }
            contador = 0;
        }
        else { 
            contador++;
        }

    }

    private void dibujarColumnas(List<char> lista, DrawingContext dc, int xPos, ref int contador) {

        if (timer.Tick())
        {
            AñadirLetraAleatoria(lista, ref contador);

            int y = 0;
            foreach (char letra in lista)
            {

                FormattedText texto = new FormattedText(
                letra.ToString(),
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                16,
                letterBrush);

                dc.DrawText(texto, new Point(xPos, y));
                y += 20; // espacio entre líneas
            }
        }

    }


    public void Render(RenderContext ctx){
        ctx.Display(
            draw: dc => {
                //Base
                var color = Color.Parse("#0F0F0F");
                var brush = new SolidColorBrush(color);
                dc.FillRectangle(brush, new Rect(ctx.Bounds.Size));


                //Columnas de Matrix
                dibujarColumnas(lista1, dc, 0, ref c1);
                Console.WriteLine(c1);
                dibujarColumnas(lista2, dc, 30, ref c2);
                Console.WriteLine(c2);
                dibujarColumnas(lista3, dc, 60, ref c3);
                Console.WriteLine(c3);
                dibujarColumnas(lista4, dc, 90, ref c4);
                Console.WriteLine(c4);

            }
        );

    }

}
