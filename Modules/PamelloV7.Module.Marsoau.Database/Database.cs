using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Plugins;
using PamelloV7.Server.Loaders;

namespace PamelloV7.Module.Marsoau.Database;

public class Database : IPamelloModule
{
    public string Name => "Database";
    public string Author => "Marsoau";
    public string Description => "Basic database repositories and other database functionality";
    public ELoadingStage Stage => ELoadingStage.Early;
    
    public void Configure(IServiceCollection services) {
        DatabaseRepositoriesLoader.Configure(services);
    }

    public void Startup(IServiceProvider services) {
        DatabaseRepositoriesLoader.Load(services).Wait();
    }
}
