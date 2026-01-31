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
    public async Task<IPamelloEntity> GetSingleRequiredAsync(string query, IPamelloUser scopeUser)
        => await GetSingleAsync(query, scopeUser) ?? throw new PamelloException($"Entity not found: {query}");

    public async Task<TPamelloEntity> GetSingleRequiredAsync<TPamelloEntity>(string query, IPamelloUser scopeUser) {
        var result = await GetSingleAsync<TPamelloEntity>(query, scopeUser);
        if (result is not null) return result;
        
        if (query.Contains('$')) throw new PamelloException($"Entity by query \"{query}\" not found");
        
        var attribute = typeof(TPamelloEntity).GetCustomAttribute<ValueEntityAttribute>();
        
        throw new PamelloException($"Entity by query \"{query}\" not found in provider \"{attribute?.ProviderName}\"");
    }
    
    //single
    public async Task<IPamelloEntity?> GetSingleAsync(string query, IPamelloUser scopeUser)
        => (await GetAsync(query, scopeUser)).FirstOrDefault();
    public async Task<TPamelloEntity?> GetSingleAsync<TPamelloEntity>(string query, IPamelloUser scopeUser)
        => (await GetAsync<TPamelloEntity>(query, scopeUser)).FirstOrDefault();
    
    //many
    public Task<List<IPamelloEntity>> GetAsync(string query, IPamelloUser scopeUser);

    public async Task<List<TPamelloEntity>> GetAsync<TPamelloEntity>(string query, IPamelloUser scopeUser) {
        if (query.Contains("$")) return (await GetAsync(query, scopeUser)).OfType<TPamelloEntity>().ToList();
        
        var attribute = typeof(TPamelloEntity).GetCustomAttribute<ValueEntityAttribute>();
        if (attribute is null) return [];
        
        return (await GetAsync($"{attribute.ProviderName}${query}", scopeUser)).OfType<TPamelloEntity>().ToList();
    }
    
    //reflective
    public async Task<object?> ReflectiveGetSingleAsync(Type entityType, string query, IPamelloUser scopeUser) {
        var listResult = await ReflectiveGetAsync(entityType, query, scopeUser) as IList;
        if (listResult is null) Debug.Assert(false, "Get should return List");
        
        return listResult.Count > 0 ? listResult[0] : null;
    }
    public async Task<object> ReflectiveGetAsync(Type entityType, string query, IPamelloUser scopeUser) {
        if (!entityType.IsAssignableTo(typeof(IPamelloEntity))) throw new PamelloException("Entity type must be a IPamelloEntity");

        var methods = typeof(IEntityQueryService).GetMethods();
        var method = methods.FirstOrDefault(m => m is { Name: nameof(GetAsync), IsGenericMethod: true });
        var generic = method!.MakeGenericMethod(entityType);
        
        var attribute = entityType.GetCustomAttribute<ValueEntityAttribute>();
        if (attribute is null) throw new PamelloException("Entity doesnt have ValueEntityAttribute");

        var result = generic.Invoke(this, [query.Contains('$') ? query : $"{attribute.ProviderName}${query}", scopeUser]);
        return result switch {
            null => [],
            Task task => (IEnumerable<IPamelloEntity>) await (dynamic)task,
            _ => (IEnumerable<IPamelloEntity>)result 
        };
    }
}
