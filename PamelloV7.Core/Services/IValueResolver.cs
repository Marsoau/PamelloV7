using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IValueResolver : IPamelloService
{
    public IPamelloEntity GetEntity(string value, IPamelloUser scopeUser);
    public TEntity GetEntity<TEntity>(string value, IPamelloUser scopeUser);
}
