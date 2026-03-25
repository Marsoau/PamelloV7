using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Consolonia;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.Setup.Screens;

namespace PamelloV7.Module.Marsoau.Setup.Services;

public class SetupService : IPamelloService
{
    private readonly IConsoloniaService _consolonia;

    public SetupService(IServiceProvider services) {
        _consolonia = services.GetRequiredService<IConsoloniaService>();
    }
    
    public async Task StartGuidedSetupAsync() {
        var screen = _consolonia.SetScreen(() => new GuidedSetupScreen());
        
        await screen.LoadingCompleted.Task;
        await screen.SetupCompleted.Task;
    }
}
