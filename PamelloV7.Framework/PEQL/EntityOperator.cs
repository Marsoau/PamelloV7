using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.PEQL;

public abstract class EntityOperator
{
    protected readonly IServiceProvider _services;

    protected readonly IEntityQueryService _peql;
    
    public EntityOperator(IServiceProvider services) {
        _services = services;
        
        _peql = services.GetRequiredService<IEntityQueryService>();
    }

    public abstract Task<IEnumerable<IPamelloEntity>> ExecuteAsync(IPamelloUser scopeUser, string query, string value);
}
