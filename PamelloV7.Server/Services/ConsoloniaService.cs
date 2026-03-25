using Avalonia.Controls;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Server.Consolonia;

namespace PamelloV7.Server.Services;

public class ConsoloniaService : IPamelloService
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

    public TUserControl SetScreen<TUserControl>()
        where TUserControl : UserControl {
        TUserControl screen;
        
        App.MainWindow.Content = screen = _services.GetRequiredService<TUserControl>();
        
        return screen;
    }
}
