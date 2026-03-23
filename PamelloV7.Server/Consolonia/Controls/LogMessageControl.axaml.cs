using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Server.Consolonia.Controls;

public partial class LogMessageControl : UserControl
{
    public LogMessageControl(object? obj, IPamelloModule? module, ELogLevel level) {
        InitializeComponent();

        var noteStack = new StackPanel {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        var levelBrush = level switch {
            ELogLevel.Log => Brushes.DarkGray,
            ELogLevel.Warning => Brushes.Yellow,
            ELogLevel.Error => Brushes.Red,
        };
        var moduleBrush = module is null ? Brushes.Violet : Brushes.Green;
            
        noteStack.Children.AddRange([
            new TextBlock {
                Text = $"{DateTime.Now:HH:mm:ss.fff}",
                Foreground = Brushes.White,
            },
            new TextBlock {
                Text = $" [",
                Foreground = Brushes.DarkGray,
            },
            new TextBlock {
                Text = $"{module?.Name ?? "Server"}",
                Foreground = moduleBrush,
            },
            new TextBlock {
                Text = $" | ",
                Foreground = Brushes.DarkGray,
            },
            new TextBlock() {
                Text = level.ToString(),
                Foreground = levelBrush,
            },
            new TextBlock {
                Text = $"] ",
                Foreground = Brushes.DarkGray,
            },
        ]);

        var text = new TextBlock {
            Text = obj?.ToString() ?? "null",
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        
        Grid.Children.Add(noteStack);
        Grid.Children.Add(text);
        
        Grid.SetColumn(noteStack, 0);
        Grid.SetColumn(text, 1);
    }
}
