using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
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
            Dispatcher.UIThread.Post(() => Box.ScrollToEnd(), DispatcherPriority.Loaded);
        }
    }
}
