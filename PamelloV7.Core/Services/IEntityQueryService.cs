using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Services;

public interface IEntityQueryService
{
    public IPamelloEntity GetSingle(string query, IPamelloUser scopeUser);
    public TPamelloEntity GetSingle<TPamelloEntity>(string query, IPamelloUser scopeUser);
    public IEnumerable<IPamelloEntity> Get(string query, IPamelloUser scopeUser);
    public IEnumerable<TPamelloEntity> Get<TPamelloEntity>(string query, IPamelloUser scopeUser);
}
