using PamelloV7.Core.Dto;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Repositories;

public interface IRemoteRepository
{
    public string ProviderName { get; }
    public string RemoteInterfaceName { get; }
    public Type DtoType { get; }
    public Type EntityType { get; }

    public IRemoteEntity Load(PamelloEntityDto dto);

    public IRemoteEntity GetSingleRequired(int id);
    public Task<IRemoteEntity> GetSingleRequiredAsync(int id);
    
    public IRemoteEntity? GetSingle(int id);
    public Task<IRemoteEntity?> GetSingleAsync(int id);
    public Task<IRemoteEntity?> GetSingleAsync(string query);
    
    public Task<IEnumerable<IRemoteEntity>> GetAsync(string query);
    public Task<IEnumerable<int>> GetIdsAsync(string query);
    
    public void ClearCache();
}
