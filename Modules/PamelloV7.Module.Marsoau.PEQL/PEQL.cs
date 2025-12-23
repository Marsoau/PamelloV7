using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Model.PEQL;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.PEQL.Services;

namespace PamelloV7.Module.Marsoau.PEQL;

public class PEQL : IPamelloModule
{
    public string Name => "PEQL";
    public string Author => "Marsoau";
    public string Description => "PEQL language implementation";
    public ELoadingStage Stage => ELoadingStage.Early;
    
    public void Startup(IServiceProvider services) {
        var collection = services.GetRequiredService<IServiceCollection>();
        var query = (EntityQueryService)services.GetRequiredService<IEntityQueryService>();
        
        query.LoadProviders(collection, services);
        query.LoadOperators(services);
    }
}
