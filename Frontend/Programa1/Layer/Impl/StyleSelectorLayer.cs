using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Programa1.layer;
using Programa1.Layer.Bridge;
using Programa1.Style;
using Programa1.Style.Background;
using Programa1.Style.Transition;

namespace Programa1.Layer;

public sealed class StyleSelectorLayer : IRenderLayer {
    
    private ComboBox? _backgroundSelector;
    private ComboBox? _transitionSelector;

    public void Render(RenderContext context, RenderStyle style) {
        context.Display(
            overlay: panel => {
                if (_backgroundSelector != null) {
                    return;
                }

                // 3 column grid (2 at the left and 1 at the right)
                Grid rootGrid = new Grid {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                
                StackPanel rightPanel = new StackPanel {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Spacing = 8
                };
                
                Grid.SetColumn(rightPanel, 2);

                // Background options
                BackgroundOption[] backgroundOptions = new[] {
                    new BackgroundOption("Default", () => new DefaultBackground()),
                    new BackgroundOption("A1", () => new BackgroundA1()),
                    new BackgroundOption("A2", () => new BackgroundA2()),
                    new BackgroundOption("Ball", () => new BackgroundBall()),
                    new BackgroundOption("Gradient", () => new BackgroundBallGradiente()),
                    new BackgroundOption("Walls", () => new BackgroundBallWalls()),
                    new BackgroundOption("Beige", () => new BeigeBackground()),
                    new BackgroundOption("Dark", () => new DarkBackground()),
                    new BackgroundOption("Dark Line", () => new DarkLineBackground()),
                    new BackgroundOption("Light Green", () => new LightGreenBackground()),
                    new BackgroundOption("Light Line", () => new LightLineBackground()),
                    new BackgroundOption("Matrix", () => new MatrixBackground()),
                    new BackgroundOption("Red", () => new RedBackground())
                };

                // Transition options
                TransitionOption[] transitionOptions = new[] {
                    new TransitionOption("Default", () => new DefaultTransition()),
                    new TransitionOption("Blinds", () => new BlindsTransition()),
                    new TransitionOption("Block Fade", () => new BlockFadeTransition()),
                    new TransitionOption("Mosaic", () => new MosaicTransition()),
                    new TransitionOption("Shape", () => new ShapeTransition()),
                    new TransitionOption("Slide", () => new SlideTransition()),
                    new TransitionOption("Spiral", () => new SpiralTransition()),
                };

                _backgroundSelector = new ComboBox {
                    ItemsSource = backgroundOptions,
                    SelectedIndex = 0,
                    Width = 180,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                _transitionSelector = new ComboBox {
                    ItemsSource = transitionOptions,
                    SelectedIndex = 0,
                    Width = 180,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Button applyBackgroundButton = new Button {
                    Content = "Apply Background",
                    Width = 180,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Button applyTransitionButton = new Button {
                    Content = "Apply Transition",
                    Width = 180,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                
                applyBackgroundButton.Click += (_, _) => {
                    if (_backgroundSelector?.SelectedItem is BackgroundOption opt) {
                        style.Background = opt.Factory();
                    }
                    
                };

                applyTransitionButton.Click += (_, _) => {
                    if (_transitionSelector?.SelectedItem is TransitionOption opt) {
                        style.Transition = opt.Factory();
                    }
                    
                };

                rightPanel.Children.Add(new TextBlock {
                    Text = "Background:",
                    FontWeight = FontWeight.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                
                rightPanel.Children.Add(_backgroundSelector);
                rightPanel.Children.Add(applyBackgroundButton);

                rightPanel.Children.Add(new TextBlock {
                    Text = "Transition:",
                    FontWeight = FontWeight.Bold,
                    Margin = new Thickness(0, 12, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                
                rightPanel.Children.Add(_transitionSelector);
                rightPanel.Children.Add(applyTransitionButton);

                rootGrid.Children.Add(rightPanel);
                panel.Children.Add(rootGrid);
            },
            once: true
        );
        
    }

    private sealed class BackgroundOption {
        public string Name { get; }
        public Func<IBackground> Factory { get; }

        public BackgroundOption(string name, Func<IBackground> factory) {
            Name = name;
            Factory = factory;
        }

        public override string ToString() => Name;
    }

    private sealed class TransitionOption {
        public string Name { get; }
        public Func<ITransition> Factory { get; }

        public TransitionOption(string name, Func<ITransition> factory) {
            Name = name;
            Factory = factory;
        }

        public override string ToString() => Name;
    }
    
}
