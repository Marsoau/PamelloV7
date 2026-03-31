using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Server.Consolonia.Controls;

public partial class RefreshableMessageControl : UserControl
{
    private RefreshableLogMessage? _currentMessage;
    
    public RefreshableMessageControl() {
        InitializeComponent();
        
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e) {
        // 1. CRITICAL: Unsubscribe from the OLD message to prevent memory leaks!
        if (_currentMessage != null)
        {
            _currentMessage.OnRefresh -= OnRefresh;
        }

        // 2. Get the NEW message that Avalonia just handed us
        _currentMessage = DataContext as RefreshableLogMessage;

        // 3. Subscribe to the new message and update the UI immediately
        if (_currentMessage != null)
        {
            _currentMessage.OnRefresh += OnRefresh;
            Refresh();
        }
    }

    private void OnRefresh() {
        Dispatcher.UIThread.InvokeAsync(Refresh);
    }

    private void Refresh() {
        if (_currentMessage == null) {
            Grid.Children.Clear();
            return;
        }
        
        var noteStack = new StackPanel {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        var levelBrush = _currentMessage.Level switch {
            ELogLevel.Log => Brushes.DarkGray,
            ELogLevel.Warning => Brushes.Yellow,
            ELogLevel.Error => Brushes.Red,
            ELogLevel.Debug => Brushes.DimGray,
            _ => throw new ArgumentOutOfRangeException()
        };
        var moduleBrush = _currentMessage.Module?.Color ?? Brushes.MediumPurple;
            
        noteStack.Children.AddRange([
            new TextBlock {
                Text = $"{_currentMessage.TimeStamp:HH:mm:ss.fff}",
                Foreground = Brushes.White,
            },
            new TextBlock {
                Text = $" [",
                Foreground = Brushes.DarkGray,
            },
            new TextBlock {
                Text = $"{_currentMessage.Module?.Name ?? "Server"}",
                Foreground = moduleBrush,
            },
            new TextBlock {
                Text = $" | ",
                Foreground = Brushes.DarkGray,
            },
            new TextBlock() {
                Text = _currentMessage.Level.ToString(),
                Foreground = levelBrush,
            },
            new TextBlock {
                Text = $"] ",
                Foreground = Brushes.DarkGray,
            },
        ]);

        var text = new TextBlock {
            Text = _currentMessage.Content,
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
