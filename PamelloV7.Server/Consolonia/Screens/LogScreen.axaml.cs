using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PamelloV7.Framework.Consolonia.Screens;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Services;
using PamelloV7.Framework.Modules;
using PamelloV7.Server.Consolonia.Controls;
using PamelloV7.Server.Logging;
using TextCopy;

namespace PamelloV7.Server.Consolonia.Screens;

public partial class LogScreen : PamelloScreen
{
    public LogScreen() {
        InitializeComponent();
        
        DataContext = Output.Logger;
        
        ((PamelloLogger)Output.Logger).Messages.CollectionChanged += MessagesOnCollectionChanged;
    }

    private void MessagesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            Dispatcher.UIThread.Post(() => MessageList.ScrollToEnd(), DispatcherPriority.Loaded);
        }
    }

    private void CopyButton_OnClick(object? sender, RoutedEventArgs e) {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.Clipboard is null) return;
        
        var sb = new StringBuilder();

        foreach (var message in ((PamelloLogger)Output.Logger).Messages) {
            sb.AppendLine(message.ToString());
        }

        try {
            var base64Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
            var osc52Sequence = $"\x1b]52;c;{base64Text}\x07";

            Console.Write(osc52Sequence);
        }
        catch (Exception ex) {
            Output.Write($"Copy Failed: {ex.Message}");
        }
    }
}
