using System.Reflection;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Query.Base;

public interface IRemoteEntityQueryService
{
    //no ids[] > entities[] because SafeStoredEntities will be used for that
    //use GetIdsAsync to get ids and make SafeStoredEntities out of them
    
    //required
    
    public TRemoteEntity GetSingleRequired<TRemoteEntity>(int id)
        where TRemoteEntity : class, IRemoteEntity
        => GetSingle<TRemoteEntity>(id) ?? throw new Exception($"Entity {typeof(TRemoteEntity)} with id {id} not found");
    public IRemoteEntity GetSingleRequired(Type type, int id)
        => GetSingle(type, id) ?? throw new Exception($"Entity {type} with id {id} not found");

    public async Task<TRemoteEntity> GetSingleRequiredAsync<TRemoteEntity>(int id)
        where TRemoteEntity : class, IRemoteEntity
        => await GetSingleAsync<TRemoteEntity>(id) ?? throw new Exception($"Entity {typeof(TRemoteEntity)} with id {id} not found");
    public async Task<IRemoteEntity> GetSingleRequiredAsync(Type type, int id)
        => await GetSingleAsync(type, id) ?? throw new Exception($"Entity {type} with id {id} not found");

    public async Task<TRemoteEntity> GetSingleRequiredAsync<TRemoteEntity>(string query)
        where TRemoteEntity : class, IRemoteEntity
        => await GetSingleAsync<TRemoteEntity>(query) ?? throw new Exception($"No entities of type {typeof(TRemoteEntity)} found by query {query}");
    public async Task<IRemoteEntity> GetSingleRequiredAsync(Type type, string query)
        => await GetSingleAsync(type, query) ?? throw new Exception($"No entities of type {type} found by query {query}");
    
    //
    
    //single by id
    public TRemoteEntity? GetSingle<TRemoteEntity>(int id)
        where TRemoteEntity : class, IRemoteEntity
        => GetSingle(typeof(TRemoteEntity), id) as TRemoteEntity;
    public IRemoteEntity? GetSingle(Type type, int id);
    
    //single async by id
    public async Task<TRemoteEntity?> GetSingleAsync<TRemoteEntity>(int id)
        where TRemoteEntity : class, IRemoteEntity
        => await GetSingleAsync(typeof(TRemoteEntity), id) as TRemoteEntity;

    public Task<IRemoteEntity?> GetSingleAsync(Type type, int id);
    
    //single async by query
    public async Task<TRemoteEntity?> GetSingleAsync<TRemoteEntity>(string query)
        where TRemoteEntity : class, IRemoteEntity
        => (await GetAsync<TRemoteEntity>(query)).FirstOrDefault();

    public Task<IRemoteEntity?> GetSingleAsync(Type type, string query);
    
    //many query
    public async Task<IEnumerable<TRemoteEntity>> GetAsync<TRemoteEntity>(string query) //of T
        where TRemoteEntity : class, IRemoteEntity
        => (await GetAsync(typeof(TRemoteEntity), query)).OfType<TRemoteEntity>();
    public async Task<IEnumerable<int>> GetIdsAsync<TRemoteEntity>(string query) //of T
        where TRemoteEntity : class, IRemoteEntity
        => await GetIdsAsync(typeof(TRemoteEntity), query);
    
    public Task<IEnumerable<IRemoteEntity>> GetAsync(Type type, string query); //many, R.GetAsync, I, [type]
    public Task<IEnumerable<int>> GetIdsAsync(Type type, string query); //many, R.GetIdsAsync, I, [type, view=Ids]
    public Task<IEnumerable<IRemoteEntity>> GetAsync(string query); //many, server, I, [view=Detailed]
    
    internal void ClearCache();
}
