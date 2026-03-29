using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config.Parts;
using PamelloV7.Framework.Consolonia;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.Setup.Pages;
using PamelloV7.Module.Marsoau.Setup.Screens;

namespace PamelloV7.Module.Marsoau.Setup.Services;

public class SetupService : IPamelloService
{
    private readonly IConsoloniaService _consolonia;
    
    public SetupScreen? Screen { get; private set; }

    public SetupService(IServiceProvider services) {
        _consolonia = services.GetRequiredService<IConsoloniaService>();
    }
    
    public async Task StartSetupAsync() {
        Screen = _consolonia.SetScreen(() => new SetupScreen());
        
        await Screen.LoadingCompleted.Task;
        //await screen.SetupCompleted.Task;
    }

    public async Task DisplayPreInitializersAsync(List<PamelloConfigPreInitializer> preInitializers) {
        await Dispatcher.UIThread.InvokeAsync(() => DisplayPreInitializersInternalAsync(preInitializers));
    }
    private async Task DisplayPreInitializersInternalAsync(List<PamelloConfigPreInitializer> preInitializers) {
        if (Screen is null) throw new PamelloException("Setup screen is not initialized");

        var page = new InitializationPage();
        
        Screen.SetPage(page);
        
        page.AddPreInitializers(preInitializers);
        
        await page.Completed.Task;
    }
}
