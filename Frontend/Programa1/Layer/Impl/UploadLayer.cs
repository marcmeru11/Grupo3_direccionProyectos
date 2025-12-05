using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Programa1.layer;
using Programa1.Style;

namespace Programa1.Layer;

public sealed class UploadLayer : IRenderLayer {
    
    // The backend's open address
    private static readonly HttpClient Http = new() {
        BaseAddress = new Uri("http://localhost:8000")
    };

    private ComboBox? _modelSelector;
    private TextBlock? _statusText;
    private Image? _resultImage;

    // Image history meant for caching data if exporting the information
    private readonly List<ProcessedImageEntry> _history = new();

    public void Render(RenderContext context, RenderStyle style) {
        context.Display(
            overlay: panel => {
                if (_statusText != null) {
                    return;
                }

                Grid rootGrid = new Grid {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                // Central panel (the two columns at the left of the window)
                StackPanel centerPanel = new StackPanel {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Spacing = 10
                };
                
                Grid.SetColumn(centerPanel, 0);
                Grid.SetColumnSpan(centerPanel, 2);

                Button button = new Button {
                    Content = "Select image",
                    Width = 105,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                _resultImage = new Image {
                    Width = 300,
                    Height = 300,
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                _statusText = new TextBlock {
                    Text = "",
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 8, 0, 0),
                    TextAlignment = TextAlignment.Center
                };

                button.Click += async (_, _) => await OnButtonClickedAsync(context.Window);

                centerPanel.Children.Add(button);
                centerPanel.Children.Add(_resultImage);
                centerPanel.Children.Add(_statusText);

                StackPanel rightPanel = new StackPanel {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Spacing = 4,
                    Margin = new Thickness(0, -180, 0, 0)
                };
                
                Grid.SetColumn(rightPanel, 2);

                TextBlock modelLabel = new TextBlock {
                    Text = "Model:",
                    FontWeight = FontWeight.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                _modelSelector = new ComboBox {
                    ItemsSource = new[] { "tensorflow", "YOLO" },
                    SelectedIndex = 0,
                    Width = 180,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                rightPanel.Children.Add(modelLabel);
                rightPanel.Children.Add(_modelSelector);

                StackPanel exportPanel = new StackPanel {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 350, 0, 0)
                };
                
                Grid.SetColumn(exportPanel, 2);

                Button exportButton = new Button {
                    Content = "Export information",
                    Width = 200,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                exportButton.Click += async (_, _) => await ExportHistoryAsync(context.Window);

                exportPanel.Children.Add(exportButton);

                rootGrid.Children.Add(centerPanel);
                rootGrid.Children.Add(rightPanel);
                rootGrid.Children.Add(exportPanel);

                panel.Children.Add(rootGrid);
            },
            once: true
        );
        
    }

    private async Task OnButtonClickedAsync(Window? owner) {
        if (owner is null || _modelSelector is null || _statusText is null || _resultImage is null) {
            return;
        }

        OpenFileDialog dialog = new OpenFileDialog {
            AllowMultiple = false,
            Filters = {
                new FileDialogFilter { Name = "Images", Extensions = { "png" } }
            }
            
        };

        String[]? files = await dialog.ShowAsync(owner);
        if (files is null || files.Length == 0)
            return;

        String path = files[0];

        try {
            _statusText.Text = "Processing image...";

            byte[] bytes = await File.ReadAllBytesAsync(path);
            string base64 = Convert.ToBase64String(bytes);
            string modelName = _modelSelector.SelectedItem?.ToString() ?? "tensorflow";

            var payload = new {
                model = modelName,
                image_base64 = base64
            };

            string json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await Http.PostAsync("/procesar", content);
            resp.EnsureSuccessStatusCode();

            string respJson = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(respJson);
            var root = doc.RootElement;

            string statusStr = root.GetProperty("status").GetString() ?? "desconocido";

            double? errorValue = null;
            double? thresholdValue = null;

            string extra = "";
            if (root.TryGetProperty("error", out var errorProperty) && root.TryGetProperty("threshold", out var thresholdProperty)) {
                errorValue = errorProperty.GetDouble();
                thresholdValue = thresholdProperty.GetDouble();
                extra = $" | Error: {errorValue.Value:F4} | Threshold: {thresholdValue.Value:F4}";
            }

            _statusText.Text = $"Model: {modelName} | State: {statusStr.ToUpper()}{extra}";

            string? imgBase64 = null;
            if (root.TryGetProperty("reconstructed_base64", out var imgProp)) {
                imgBase64 = imgProp.GetString();
            }
            else if (root.TryGetProperty("reconstructed_base", out var img2Prop)) {
                imgBase64 = img2Prop.GetString();
            }

            if (!string.IsNullOrEmpty(imgBase64)) {
                byte[] imgBytes = Convert.FromBase64String(imgBase64);
                using var ms = new MemoryStream(imgBytes);
                _resultImage.Source = new Bitmap(ms);
            }

            _history.Add(new ProcessedImageEntry {
                Timestamp = DateTime.Now,
                ImageName = Path.GetFileName(path),
                Model = modelName,
                Status = statusStr,
                Error = errorValue,
                Threshold = thresholdValue
            });
            
        }
        catch (Exception exception) {
            _statusText.Text = $"Error: {exception.Message}";
        }
        
    }

    private async Task ExportHistoryAsync(Window? owner) {
        if (owner is null || _history.Count == 0) {
            return;
        }

        SaveFileDialog dialog = new SaveFileDialog {
            DefaultExtension = "txt",
            InitialFileName = "processed_data.txt",
            Filters = {
                new FileDialogFilter { Name = "Text files", Extensions = { "txt" } }
            }
            
        };

        var path = await dialog.ShowAsync(owner);
        if (string.IsNullOrWhiteSpace(path)) {
            return;
        }

        List<string> lines = new List<string>();
        foreach (var entry in _history) {
            var extra = $"Model={entry.Model};Status={entry.Status}";
            if (entry.Error.HasValue && entry.Threshold.HasValue) {
                extra += $";Error={entry.Error.Value:F4};Threshold={entry.Threshold.Value:F4}";
            }

            string line = $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss}; {entry.ImageName}; {extra}";
            lines.Add(line);
        }

        await File.WriteAllLinesAsync(path, lines);
    }

    private sealed class ProcessedImageEntry {
        public DateTime Timestamp { get; set; }
        public string ImageName { get; set; } = "";
        public string Model { get; set; } = "";
        public string Status { get; set; } = "";
        public double? Error { get; set; }
        public double? Threshold { get; set; }
    }
    
}
