using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PamelloV7.Server.Consolonia.Screens;
using PamelloV7.Server.Consolonia.Windows;

namespace PamelloV7.Server.Consolonia;

public partial class ConsoloniaApp : Application
{
    public IServiceProvider Services {
        get => field ?? throw new InvalidOperationException("Services not set");
        internal set;
    }

    public MainScreen MainScreen {
        private set; get => field ??= Dispatcher.UIThread.Invoke(() => new MainScreen(Services));
    }

    public LogScreen LogScreen {
        private set; get => field ??= Dispatcher.UIThread.Invoke(() => new LogScreen());
    }

    public readonly TaskCompletionSource Started = new();

    public MainWindow MainWindow { get; private set; } = null!;
    
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktopLifetime) return;
        
        desktopLifetime.MainWindow = MainWindow = new MainWindow();
        SetLogScreen();
        
        base.OnFrameworkInitializationCompleted();
        
        Started.SetResult();
    }

    public void SetMainScreen() {
        SetScreen(MainScreen);
    }
    public void SetLogScreen() {
        SetScreen(LogScreen);
    }

    public void SetScreen(Control? screen) {
        Dispatcher.UIThread.InvokeAsync(() => {
            if (MainWindow.Content is MainScreen) {
                MainScreen = null!;
            }
            else if (MainWindow.Content is LogScreen) {
                LogScreen = null!;
            }

            MainWindow.Content = screen;
        });
    }
}
