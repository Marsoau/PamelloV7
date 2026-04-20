using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Server.Loaders;

namespace PamelloV7.Module.Marsoau.Database;

public class Database : IPamelloModule
{
    public string Name => "Database";
    public string Author => "Marsoau";
    public string Description => "Basic database repositories and other database functionality";
    public ELoadingStage Stage => ELoadingStage.Database;
    public int Color => 0xB5A8EF;

    public async Task StartupAsync(IServiceProvider services) {
        var peql = services.GetRequiredService<IEntityQueryService>();
        SafeStoredEntityStaticContainer.GetById = (type, id) => peql.GetById(type, id);
        
        var collection = services.GetRequiredService<IServiceCollection>();
        
        DatabaseRepositoriesLoader.Load(collection, services).Wait();
    }
}
