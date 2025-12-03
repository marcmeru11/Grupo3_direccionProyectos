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

public class MatrixBackground : IBackground
{
    private readonly Timer timer = new Timer(1); // El argumento entre 60 (fps) son los segundos (30/60 = 0.5s)
    private static Random random = new Random();

    //Dinamic lists to hold the columns and their counters
    private List<List<char>> columnas = new List<List<char>>();
    private List<int> contadores = new List<int>();
    private List<int> offsets = new List<int>();
    private List<int> delays = new List<int>();


    private SolidColorBrush letterBrush = new SolidColorBrush(Color.Parse("#2dc724"));

    // "añadir letra aleatoria" means "add random letter" (for erasmus students)
    private void AnadirLetraAleatoria(List<char> listaActual)
    {
        int lenght = listaActual.Count;
        if (lenght == 0) lenght = random.Next(3, 15); // Init if the list is empty

        for (int a = 0; a < lenght; a++)
        {
            // Selects a random letter from the MatrixLetters list
            int index = random.Next(MatrixLetters.Lista.Length);
            char letra = MatrixLetters.Lista[index];

            // Adds (or updates) the letter to the current list
            if (listaActual.Count > a)
            {
                listaActual[a] = letra;
            }
            else
            {
                listaActual.Add(letra);
            }
        }
    }

    // "dibuja columnas" means "draw columns" (for erasmus students)
    private void dibujarColumnas(List<char> lista, DrawingContext dc, int xPos, int indiceColumna, double maxHeight)
    {
        if (delays[indiceColumna] > 0)
        {
            // Todavía está en espera, reducimos el contador y no dibujamos nada
            delays[indiceColumna]--;
            return;
        }

        if (timer.Tick())
        {
            // Updates the counter for the current column
            contadores[indiceColumna]++;
            if (contadores[indiceColumna] >= 15)
            {
                AnadirLetraAleatoria(lista);
                contadores[indiceColumna] = 0;
            }

            // Formats the characters and draws them on the screen
            int y = offsets[indiceColumna];
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
                y += 20; // Vertical spacing between letters (hardcoded UwU)
            }

            offsets[indiceColumna] += 2; // Speed of the falling letters
            if(offsets[indiceColumna] > maxHeight)
            {

                delays[indiceColumna] = random.Next(100, 300);
                offsets[indiceColumna] = - 500; // Reset offset if it exceeds the screen height
            }
        }
    }

    public void Render(RenderContext ctx)
    {
        ctx.Display(
            draw: dc =>
            {
                // Base
                var color = Color.Parse("#0F0F0F");
                var brush = new SolidColorBrush(color);
                dc.FillRectangle(brush, new Rect(ctx.Bounds.Size));

                // Calculamos cuántas columnas caben en el ancho de la pantalla
                int numeroColumnas = (int)(ctx.Bounds.Width / 30);

                //Adjust the number of columns to match the screen width (bigger screen, more columns)
                while (columnas.Count < numeroColumnas)
                {
                    columnas.Add(new List<char>());
                }
                while (columnas.Count > numeroColumnas)
                {
                    columnas.RemoveAt(columnas.Count - 1);
                }

                // Make sure the counters list matches the number of columns
                while (contadores.Count < columnas.Count)
                {
                    contadores.Add(0); // Counter init
                }
                while (contadores.Count > columnas.Count)
                {
                    contadores.RemoveAt(contadores.Count - 1); // Delete extra counters (if screen size decreases)
                }

                //The same with the offsets list
                while (offsets.Count < columnas.Count)
                {
                    offsets.Add(0); // each column inits at 0
                }
                while (offsets.Count > columnas.Count)
                {
                    offsets.RemoveAt(offsets.Count - 1);
                }

                //The delays make the columns start at different times
                while (delays.Count < columnas.Count)
                {
                    delays.Add(random.Next(200, 600)); // each column waits between arg1 and arg2 frames to start
                }
                while (delays.Count > columnas.Count)
                {
                    delays.RemoveAt(delays.Count - 1);
                }


                // Dibujar todas las columnas
                for (int i = 0; i < columnas.Count; i++)
                {
                    dibujarColumnas(columnas[i], dc, i * 30, i, ctx.Bounds.Height); // Index * 30 to space columns horizontally (30 is also hardcoded hehe)
                }

                
            }
        );
    }
}
