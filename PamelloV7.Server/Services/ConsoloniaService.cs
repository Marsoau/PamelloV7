using Avalonia.Controls;
using Avalonia.Threading;
using PamelloV7.Framework.Consolonia;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Server.Consolonia;

namespace PamelloV7.Server.Services;

public class ConsoloniaService : IConsoloniaService
{
    private readonly IServiceProvider _services;

    private ConsoloniaApp? _app;
    private ConsoloniaApp App => _app ?? throw new InvalidOperationException("Consolonia app not set");
    
    public bool IsInitialized => _app != null;
    
    public ConsoloniaService(IServiceProvider services) {
        _services = services;
    }
    
    internal void SetApp(ConsoloniaApp? app) {
        if (app is null) return;
        
        _app = app;
        _app.Services = _services;
    }

    public bool IsAvailable => _app is not null;

    public void SetMainScreen() {
        App.SetMainScreen();
    }
    public void SetLogScreen() {
        App.SetLogScreen();
    }

    public TControl SetScreen<TControl>(Func<TControl> getScreen) where TControl : Control {
        var screen = Dispatcher.UIThread.Invoke(getScreen);
        
        App.SetScreen(screen);
        
        return screen;
    }

    public void SetScreen(Control screen) {
        App.SetScreen(screen);
    }
}
