using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using PamelloV7.Framework.Modules;
using PamelloV7.Server.Consolonia.Controls;
using PamelloV7.Server.Consolonia.Screens.Base;

namespace PamelloV7.Server.Consolonia.Screens;

public partial class LoadingScreen : UserControl, ILoggingScreen
{
    public readonly TaskCompletionSource Started = new();
    
    public ObservableCollection<LogMessageControl> Messages { get; } = [];
    
    public LoadingScreen() {
        InitializeComponent();
        
        DataContext = this;
        Loaded += (_, _) => Started.SetResult();
    }

    public void WriteLine(object? obj, IPamelloModule? module, ELogLevel level) {
        Dispatcher.UIThread.Invoke(() => {
            Messages.Add(new LogMessageControl(obj, module, level) { HorizontalAlignment = HorizontalAlignment.Stretch });
        });
    }
}
