using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PamelloV7.Server.Consolonia.Windows;
using LoadingScreen = PamelloV7.Server.Consolonia.Screens.LoadingScreen;

namespace PamelloV7.Server.Consolonia;

public partial class ConsoloniaApp : Application
{
    private IServiceProvider _services { get; set; } = null!;
    
    public MainWindow MainWindow { get; private set; } = null!;
    
    public LoadingScreen LoadingScreen { get; private set; } = null!;

    public readonly TaskCompletionSource Started = new();
    
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktopLifetime) return;
        
        desktopLifetime.MainWindow = MainWindow = new MainWindow();
        SetScreen(LoadingScreen = new LoadingScreen());
        
        base.OnFrameworkInitializationCompleted();
        
        Started.SetResult();
    }

    public void SetServices(IServiceProvider services) {
        _services = services;
    }

    public void SetScreen(UserControl screen) {
        MainWindow.Content = screen;
    }
}
