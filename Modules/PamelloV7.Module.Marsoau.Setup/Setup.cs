using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Modules;
using PamelloV7.Module.Marsoau.Setup.Services;

namespace PamelloV7.Module.Marsoau.Setup;

public class Setup : IPamelloModule
{
    public string Name => "Setup";
    public string Author => "Marsoau";
    public string Description => "Automated setup module";
    public ELoadingStage Stage => ELoadingStage.Setup;

    public async Task StartupAsync(IServiceProvider services) {
        if (false) return; //check if setup is needed
        
        var setup = services.GetRequiredService<SetupService>();
        
        Console.Clear();
        Console.WriteLine("Setup module started");
        Console.ReadLine();
    }
}
