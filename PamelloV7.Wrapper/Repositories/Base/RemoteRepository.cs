using System.Reflection;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Containers;
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
    public Type EntityType => typeof(TEntityType);

    public RemoteRepository(PamelloRequestsService requests) {
        _requests = requests;

        _loaded = [];
        
        var attribute = typeof(TEntityType).GetCustomAttribute<RemoteEntityInfoAttribute>();
        if (attribute is null) throw new Exception($"no required attribute RemoteEntityAttribute on remote entity {typeof(TEntityType).Name}");
        
        ProviderName = attribute.ProviderName;
        RemoteInterfaceName = attribute.RemoteInterfaceName;
        DtoType = attribute.DtoType;
    }
    
    IRemoteEntity IRemoteRepository.Load(PamelloEntityDto dto) => Load(dto);
    public TEntityType Load(PamelloEntityDto dto) {
        var entity = GetSingle(dto.Id);
        if (entity is not null) return entity;

        entity = Activator.CreateInstance(typeof(TEntityType), dto) as TEntityType;
        if (entity is null) throw new Exception($"could not create instance of {typeof(TEntityType).Name}");
        
        _loaded.Add(entity);
        
        return entity;
    }
    
    public TEntityType? GetSingle(int id) {
        return _loaded.FirstOrDefault(x => x.Id == id);
    }

    public async Task<TEntityType?> GetSingleAsync(int id) {
        var entity = GetSingle(id);
        if (entity is not null) return entity;

        return await GetSingleAsync(id.ToString());
    }

    public async Task<TEntityType?> GetSingleAsync(string query) {
        var result = await _requests.GetEntitiesAsync(DtoType, $"{ProviderName}${query}");
        if (result.FirstOrDefault() is not { } firstDto) return null;
        
        return Load(firstDto);
    }

    public async Task<IEnumerable<TEntityType>> GetAsync(string query) {
        var results = await _requests.GetEntitiesAsync(DtoType, $"{ProviderName}${query}");
        return results.Select(Load).ToList();
    }
    public async Task<IEnumerable<int>> GetIdsAsync(string query) {
        return await _requests.GetEntitiesIdsAsync($"{ProviderName}${query}");
    }

    //interface
    
    public IRemoteEntity GetSingleRequired(int id)
        => GetSingle(id) ?? throw new PamelloException($"{EntityType.Name} with id {id} not found");
    public async Task<IRemoteEntity> GetSingleRequiredAsync(int id)
        => await GetSingleAsync(id) ?? throw new PamelloException($"{EntityType.Name} with id {id} not found");

    IRemoteEntity? IRemoteRepository.GetSingle(int id) => GetSingle(id);
    async Task<IRemoteEntity?> IRemoteRepository.GetSingleAsync(int id) => await GetSingleAsync(id);
    async Task<IRemoteEntity?> IRemoteRepository.GetSingleAsync(string query) => await GetSingleAsync(query);

    async Task<IEnumerable<IRemoteEntity>> IRemoteRepository.GetAsync(string query) => await GetAsync(query);

    Task<IEnumerable<int>> IRemoteRepository.GetIdsAsync(string query) => GetIdsAsync(query);

    public void ClearCache() {
        _loaded.Clear();
    }
}
