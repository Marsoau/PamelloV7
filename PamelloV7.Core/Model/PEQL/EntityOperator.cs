using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.Model.PEQL;

public abstract class EntityOperator
{
    protected readonly IServiceProvider _services;

    protected readonly IEntityQueryService _peql;
    
    public EntityOperator(IServiceProvider services) {
        _services = services;
        
        _peql = services.GetRequiredService<IEntityQueryService>();
    }

    public abstract IEnumerable<IPamelloEntity> Execute(IPamelloUser scopeUser, string query, string value);
}
