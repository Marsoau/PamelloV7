using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Modules;
using PamelloV7.Server.Loaders;

namespace PamelloV7.Module.Marsoau.Database;

public class Database : IPamelloModule
{
    public string Name => "Database";
    public string Author => "Marsoau";
    public string Description => "Basic database repositories and other database functionality";
    public ELoadingStage Stage => ELoadingStage.Earliest;

    public void Startup(IServiceProvider services) {
        var collection = services.GetRequiredService<IServiceCollection>();
        
        DatabaseRepositoriesLoader.Load(collection, services).Wait();
    }
}
