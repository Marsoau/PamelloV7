using Avalonia.Controls;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Server.Consolonia;

namespace PamelloV7.Server.Services;

public class ConsoloniaService : IPamelloService
{
    private readonly IServiceProvider _services;

    private ConsoloniaApp? _app;
    private ConsoloniaApp App => _app ?? throw new InvalidOperationException("Consolonia app not set");
    
    public ConsoloniaService(IServiceProvider services) {
        _services = services;
    }
    
    internal void SetApp(ConsoloniaApp app) {
        _app = app;
        _app.SetServices(_services);
    }

    public TUserControl SetScreen<TUserControl>()
        where TUserControl : UserControl {
        TUserControl screen;
        
        App.MainWindow.Content = screen = _services.GetRequiredService<TUserControl>();
        
        return screen;
    }
}
