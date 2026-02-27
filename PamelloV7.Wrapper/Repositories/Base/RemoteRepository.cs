using System.Reflection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Entities.Attributes;
using PamelloV7.Wrapper.Entities.Base;
using PamelloV7.Wrapper.Requests;

namespace PamelloV7.Wrapper.Repositories.Base;

public class RemoteRepository<TEntityType> : IRemoteRepository
    where TEntityType : class, IRemoteEntity
{
    private readonly PamelloRequestsService _requests;
    
    private readonly List<TEntityType> _loaded;
    
    public string ProviderName { get; }
    public string RemoteInterfaceName { get; }
    public Type DtoType { get; }
    Type IRemoteRepository.EntityType => typeof(TEntityType);
    
    public RemoteRepository(PamelloRequestsService requests) {
        _requests = requests;

        _loaded = [];
        
        var attribute = typeof(TEntityType).GetCustomAttribute<RemoteEntityInfoAttribute>();
        if (attribute is null) throw new Exception($"no required attribute RemoteEntityAttribute on remote entity {typeof(TEntityType).Name}");
        
        ProviderName = attribute.ProviderName;
        RemoteInterfaceName = attribute.RemoteInterfaceName;
        DtoType = attribute.DtoType;
    }

    IRemoteEntity? IRemoteRepository.Get(int id) => Get(id);
    public TEntityType GetRequired(int id)
        => Get(id) ?? throw new PamelloException($"{typeof(TEntityType).Name} with id {id} not found");
    public TEntityType? Get(int id) {
        var entity = _loaded.FirstOrDefault(entity => entity.Id == id);
        if (entity is not null) return entity;
        
        return null;
    }
    
    public async Task<TEntityType> GetSingleRequiredAsync(int id)
        => await GetSingleAsync(id) ?? throw new PamelloException($"{typeof(TEntityType).Name} with id {id} not found");

    public async Task<TEntityType?> GetSingleAsync(int id) {
        var entity = Get(id);
        if (entity is not null) return entity;
        
        return await GetSingleAsync($"{id}");
    }

    public async Task<TEntityType?> GetSingleAsync(string query) {
        return (await GetAsync(query)).FirstOrDefault();
    }
    
    public async Task<List<TEntityType?>> GetAsync(string query) {
        var dto = (await _requests.GetEntitiesAsync(DtoType, $"{ProviderName}${query}")).FirstOrDefault();
        if (dto is null) return null;
        
        var entity = Get(dto.Id);
        if (entity is not null) return [entity];
        
        entity = Activator.CreateInstance(typeof(TEntityType), dto) as TEntityType;
        
        if (entity is not null) _loaded.Add(entity);
        
        return [entity];
    }
}
