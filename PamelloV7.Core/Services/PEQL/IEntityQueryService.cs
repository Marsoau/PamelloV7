using System.Reflection;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services.PEQL;

public interface IEntityQueryService : IPamelloService
{
    public IPamelloEntity GetSingleRequired(string query, IPamelloUser scopeUser)
        => GetSingle(query, scopeUser) ?? throw new InvalidOperationException($"Entity not found: {query}");
    public TPamelloEntity GetSingleRequired<TPamelloEntity>(string query, IPamelloUser scopeUser)
        => GetSingle<TPamelloEntity>(query, scopeUser) ?? throw new InvalidOperationException($"Entity not found: {query}");
    public IPamelloEntity? GetSingle(string query, IPamelloUser scopeUser)
        => Get(query, scopeUser).FirstOrDefault();
    public TPamelloEntity? GetSingle<TPamelloEntity>(string query, IPamelloUser scopeUser)
        => Get<TPamelloEntity>(query, scopeUser).FirstOrDefault();
    public IEnumerable<IPamelloEntity> Get(string query, IPamelloUser scopeUser);

    public IEnumerable<TPamelloEntity> Get<TPamelloEntity>(string query, IPamelloUser scopeUser) {
        if (query.Contains("$")) return Get(query, scopeUser).OfType<TPamelloEntity>();
        
        var attribute = typeof(TPamelloEntity).GetCustomAttribute<ValueEntityAttribute>();
        if (attribute is null) return [];
        
        return Get($"{attribute.ProviderName}${query}", scopeUser).OfType<TPamelloEntity>();
    }
}
