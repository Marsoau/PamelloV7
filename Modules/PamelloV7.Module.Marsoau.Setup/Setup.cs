using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Config.Loaders;
using PamelloV7.Framework.Config.Parts;
using PamelloV7.Framework.Consolonia;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Modules.Loaders;
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
        
        var config = services.GetRequiredService<IPamelloConfigLoader>();
        var modules = services.GetRequiredService<IPamelloModuleLoader>();

        List<PamelloConfigPreInitializer> preInitializers = [];

        Output.Write($"Modules ({modules.Containers.Count} containers)");
        foreach (var container in modules.Containers) {
            Output.Write($"| {container.Module.Name}{
                (container.ConfigTypes.Count > 0 ? $" ({container.ConfigTypes.Count} configs)" : "")
            }");
            
            var parts = config.Parts.Where(part => part.Module == container.Module);
            var moduleName = container.Module.ToString()!;
            
            foreach (var part in parts) {
                Output.Write($"|    {part.Name}{
                    (part.PreInitializers.Count > 0 ? $" ({part.PreInitializers.Count} pre-initializers)" : "")
                }");
                
                preInitializers.AddRange(part.PreInitializers);
            }
        }

        if (preInitializers.Count > 0) throw new ModuleStartupException(this, $"Some of the pre-initializers are not set\n{string.Join(
            "\n", preInitializers.Select(preInitializer => $"| {preInitializer.Part.Name} - {preInitializer}")
        )}");
        
        return;
        
        var setup = services.GetRequiredService<SetupService>();
        var consolonia = services.GetRequiredService<IConsoloniaService>();
        
        Output.Write("Starting guided setup");
        await setup.StartGuidedSetupAsync();
        Output.Write("Setup completed");
        
        consolonia.SetLogScreen();
    }
}
