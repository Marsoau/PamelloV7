using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.PEQL.Services;

namespace PamelloV7.Module.Marsoau.PEQL;

public class PEQL : IPamelloModule
{
    public string Name => "PEQL";
    public string Author => "Marsoau";
    public string Description => "PEQL language implementation";
    public ELoadingStage Stage => ELoadingStage.Early;
    
    public async Task StartupAsync(IServiceProvider services) {
        var collection = services.GetRequiredService<IServiceCollection>();
        var query = (EntityQueryService)services.GetRequiredService<IEntityQueryService>();
        
        query.LoadProviders(collection, services);
        query.LoadOperators(services);
    }
}
