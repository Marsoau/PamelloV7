using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Consolonia;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Module.Marsoau.Setup.Services;

namespace PamelloV7.Module.Marsoau.Setup;

public class Setup : IPamelloModule
{
    public string Name => "Setup";
    public string Author => "Marsoau";
    public string Description => "Automated setup module";
    public ELoadingStage Stage => ELoadingStage.Setup;
    public IBrush Color => Brushes.OrangeRed;

    public async Task StartupAsync(IServiceProvider services) {
        if (false) return; //check if setup is needed
        
        var setup = services.GetRequiredService<SetupService>();
        var consolonia = services.GetRequiredService<IConsoloniaService>();
        
        Output.Write("Starting guided setup");
        await setup.StartGuidedSetupAsync();
        Output.Write("Setup completed");
        
        consolonia.SetLogScreen();
    }
}
