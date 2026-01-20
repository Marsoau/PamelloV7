using System.Collections;
using System.Diagnostics;
using System.Reflection;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services.PEQL;

public interface IEntityQueryService : IPamelloService
{
    //required
    public IPamelloEntity GetSingleRequired(string query, IPamelloUser scopeUser)
        => GetSingle(query, scopeUser) ?? throw new InvalidOperationException($"Entity not found: {query}");
    public TPamelloEntity GetSingleRequired<TPamelloEntity>(string query, IPamelloUser scopeUser)
        => GetSingle<TPamelloEntity>(query, scopeUser) ?? throw new InvalidOperationException($"Entity not found: {query}");
    
    //single
    public IPamelloEntity? GetSingle(string query, IPamelloUser scopeUser)
        => Get(query, scopeUser).FirstOrDefault();
    public TPamelloEntity? GetSingle<TPamelloEntity>(string query, IPamelloUser scopeUser)
        => Get<TPamelloEntity>(query, scopeUser).FirstOrDefault();
    
    //many
    public List<IPamelloEntity> Get(string query, IPamelloUser scopeUser);

    public List<TPamelloEntity> Get<TPamelloEntity>(string query, IPamelloUser scopeUser) {
        if (query.Contains("$")) return Get(query, scopeUser).OfType<TPamelloEntity>().ToList();
        
        var attribute = typeof(TPamelloEntity).GetCustomAttribute<ValueEntityAttribute>();
        if (attribute is null) return [];
        
        return Get($"{attribute.ProviderName}${query}", scopeUser).OfType<TPamelloEntity>().ToList();
    }
    
    //reflective
    public object? GetSingle(Type entityType, string query, IPamelloUser scopeUser) {
        var listResult = Get(entityType, query, scopeUser) as IList;
        if (listResult is null) Debug.Assert(false, "Get should return List");
        
        return listResult.Count > 0 ? listResult[0] : null;
    }
    public object Get(Type entityType, string query, IPamelloUser scopeUser) {
        if (!entityType.IsAssignableTo(typeof(IPamelloEntity))) throw new ArgumentException("Entity type must be a IPamelloEntity");

        var methods = typeof(IEntityQueryService).GetMethods();
        var method = methods.FirstOrDefault(m => m is { Name: nameof(Get), IsGenericMethod: true });
        var generic = method!.MakeGenericMethod(entityType);
        
        var attribute = entityType.GetCustomAttribute<ValueEntityAttribute>();
        if (attribute is null) throw new PamelloException("Entity doesnt have ValueEntityAttribute");

        if (query.Contains("$")) return generic.Invoke(this, [query, scopeUser])!;
        return generic.Invoke(this, [$"{attribute.ProviderName}${query}", scopeUser])!;
    }
}
