using System.Reflection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Entities.Attributes;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Repositories;

public class RemoteRepository<TEntityType>
    where TEntityType : class, IPamelloEntity
{
    private readonly PamelloClient _client;
    
    private readonly List<TEntityType> _loaded;
    
    public string ProviderName { get; }
    public string RemoteInterfaceName { get; }
    public Type DtoType { get; }
    
    public RemoteRepository(PamelloClient client) {
        _client = client;

        _loaded = [];
        
        var attribute = typeof(TEntityType).GetCustomAttribute<RemoteEntityInfoAttribute>();
        if (attribute is null) throw new Exception($"no required attribute RemoteEntityAttribute on remote entity {typeof(TEntityType).Name}");
        
        ProviderName = attribute.ProviderName;
        RemoteInterfaceName = attribute.RemoteInterfaceName;
        DtoType = attribute.DtoType;
    }
    
    public TEntityType GetRequired(int id)
        => Get(id) ?? throw new PamelloException($"{typeof(TEntityType).Name} with id {id} not found");
    public TEntityType? Get(int id) {
        var entity = _loaded.FirstOrDefault(entity => entity.Id == id);
        if (entity is not null) return entity;
        
        return null;
    }
    
    public async Task<TEntityType?> GetAsync(int id) {
        var entity = Get(id);
        if (entity is not null) return entity;

        var dto = await _client.Requests.GetEntitiesAsync(DtoType, $"{ProviderName}${id}?type={RemoteInterfaceName}");
        if (entity is not null) _loaded.Add(entity);
        
        return entity;
    }
}
