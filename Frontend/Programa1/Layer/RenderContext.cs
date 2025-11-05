using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Programa1.layer;

/**
 * Renderer wrapper that contains all the relevant info from the tooltip context. It also exports multiple helpers
 * for handling gfx in an easier way. This object is passed through every single rendering layer (hence the name)
 */
public class RenderContext {
    
        public Window? Window { get; }
        public DrawingContext DrawingContext { get; private set; } = null!;
        public Rect Bounds { get; private set; }

        public Rect LeftBounds { get; private set; }
        public Rect RightBounds { get; private set; }

        // Root panel
        private Panel? _overlay;
        
        // Fix for a hard crash caused by populating a filled stack when trying to add multiple items (inside the Avalonia's visual ctx).
        // This was caused when adding 2 or more items in different stacks within the same class
        private readonly Dictionary<string, Panel> _scopes = new();
        private readonly HashSet<string> _once = new();

        private RenderContext(Window? window) {
            Window = window;
        }
        
        public static RenderContext Of(Window? window) {
            return new RenderContext(window);
        } 

        public RenderContext Begin(DrawingContext drawingContext) {
            DrawingContext = drawingContext;
            Bounds = Window is null ? new Rect() : new Rect(Window.Bounds.Size);

            double leftWidth = Bounds.Width * 0.75;
            double rightWidth = Bounds.Width * 0.25;

            LeftBounds = new Rect(Bounds.X, Bounds.Y, leftWidth, Bounds.Height);
            RightBounds = new Rect(Bounds.X + leftWidth, Bounds.Y, rightWidth, Bounds.Height);

            _overlay ??= ResolveOverlay(Window);
            
            return this;
        }

        /// <summary>
        /// Main graphics rendering abstraction. This method SHOULD be called when rendering any item within the screen (instead of adding a generic child
        /// to the root grid of the window). There are two relevant functional lambda calls:
        /// - draw: Immediate rendering of pictures and helper Avalonia geo drawings (using the lambda param). These can be either 2D or 3D (altho you'll have to simulate 3D rendering)
        /// - overlay: Build against the root panel and create minimal children branches with predefined content
        /// The actual implementation is irrelevant for the common use-case
        /// </summary>
        public void Display(Action<DrawingContext>? draw = null, Action<Panel>? overlay = null, int z = 0, bool once = false, double angleDeg = 0, double? pivotX = null, double? pivotY = null, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
            // Immediate drawing with a rotation helper (if there is any)
            if (draw is not null) {

                if (angleDeg != 0 && pivotX.HasValue && pivotY.HasValue) {
                    double cx = pivotX.Value;
                    double cy = pivotY.Value;

                    using (DrawingContext.PushTransform(Matrix.CreateTranslation(-cx, -cy)))
                    using (DrawingContext.PushTransform(Matrix.CreateRotation(angleDeg * Math.PI / 180.0)))
                    using (DrawingContext.PushTransform(Matrix.CreateTranslation(cx, cy))) {
                        draw(DrawingContext);
                    }
                    
                }
                else {
                    draw(DrawingContext);
                }
                
            }

            if (overlay is null || Window is null) {
                return;
            }

            // Scoping (implicit checksum) to handle multiple items rendering at the same time
            // https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/Primitives/AdornerLayer.cs
            String key = $"{file}:{line}";
            Dispatcher.UIThread.Post(() => {
                if (_overlay is null) {
                    return;
                }

                if (once && !_once.Add(key)) {
                    return;
                }

                if (!_scopes.TryGetValue(key, out Panel? panel)) {
                    panel = new Grid { Name = $"scope:{key}" };
                    panel.ZIndex = z;
                    _overlay.Children.Add(panel);
                    _scopes[key] = panel;
                }
                else {
                    panel.ZIndex = z;
                }

                panel.Children.Clear();
                overlay(panel);
            });
        }

        private static Panel? ResolveOverlay(Window? win) {
            if (win?.Content is Panel root) {
                return root.Children.OfType<Panel>().FirstOrDefault(p => p.Name == "OverlayRoot") ?? root; 
            }
            
            return null;
        }
        
}