using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Config.Loaders;
using PamelloV7.Framework.Config.Parts;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Modules.Loaders;

namespace PamelloV7.Module.Marsoau.Setup;

public class Setup : IPamelloModule
{
    public string Name => "Setup";
    public string Author => "Marsoau";
    public string Description => "Automated setup module";
    public ELoadingStage Stage => ELoadingStage.Setup;
    public int Color => 0xFF4B33;

    public async Task StartupAsync(IServiceProvider services) {
        var config = services.GetRequiredService<IPamelloConfigLoader>();
        var modules = services.GetRequiredService<IPamelloModuleLoader>();

        List<PamelloConfigPreInitializer> preInitializers = [];

        Output.Write($"Modules ({modules.Containers.Count} containers)");
        foreach (var container in modules.Containers) {
            Output.Write($"| {container.Module.Name}{
                (container.ConfigTypes.Count > 0 ? $" ({container.ConfigTypes.Count} configs)" : "")
            }");
            
            var parts = config.Parts.Where(part => part.Module == container.Module);
            
            foreach (var part in parts) {
                Output.Write($"|    {part.Name}{
                    (part.PreInitializers.Count > 0 ? $" ({part.PreInitializers.Count} pre-initializers)" : "")
                }");
                
                preInitializers.AddRange(part.PreInitializers);
            }
        }

        if (preInitializers.Count == 0) return;
        
        if (preInitializers.Any(initializer => !initializer.IsPreInitialized))
            throw new ModuleStartupException(this, $"Some config pre-initializers are not set:\n{string.Join("\n", preInitializers.Select(
                preInitializer => $"| {preInitializer} in {preInitializer.Part}"
            ))}");
        
        Output.Write("Setup completed");
    }
}
