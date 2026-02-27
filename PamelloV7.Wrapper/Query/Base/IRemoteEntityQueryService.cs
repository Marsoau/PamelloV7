using System.Reflection;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Query.Base;

public interface IRemoteEntityQueryService
{
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
    
    //single from loaded, no server query
    public TRemoteEntity? GetSingle<TRemoteEntity>(int id)
        where TRemoteEntity : class, IRemoteEntity
        => GetSingle(typeof(TRemoteEntity), id) as TRemoteEntity;
    public IRemoteEntity? GetSingle(Type type, int id);
    
    //many from loaded, no server query
    public IEnumerable<TRemoteEntity> Get<TRemoteEntity>(IEnumerable<int> ids)
        where TRemoteEntity : class, IRemoteEntity;

    public ISafeStoredEntities Get(Type type, IEnumerable<int> ids) {
        var method = typeof(IRemoteEntityQueryService).GetMethod(nameof(Get), BindingFlags.Instance | BindingFlags.Public)!;
        return (ISafeStoredEntities)method.MakeGenericMethod(type).Invoke(this, [ids])!;
    }
    
    //single from loaded, if not found, a server query is executed
    public async Task<TRemoteEntity?> GetSingleAsync<TRemoteEntity>(int id)
        where TRemoteEntity : class, IRemoteEntity
        => await GetSingleAsync(typeof(TRemoteEntity), id) as TRemoteEntity;

    public async Task<IRemoteEntity?> GetSingleAsync(Type type, int id) {
        var entity = GetSingle(type, id);
        return entity ?? await GetSingleAsync(type, id.ToString());
    }
    
    //single from many query
    public async Task<TRemoteEntity?> GetSingleAsync<TRemoteEntity>(string query)
        where TRemoteEntity : class, IRemoteEntity
        => (await GetAsync<TRemoteEntity>(query)).FirstOrDefault();

    public async Task<IRemoteEntity?> GetSingleAsync(Type type, string query)
        => (await GetAsync(type, query)).Entities.FirstOrDefault() as IRemoteEntity;

    //many query
    public Task<IEnumerable<TRemoteEntity>> GetAsync<TRemoteEntity>(string query)
        where TRemoteEntity : class, IRemoteEntity;

    public async Task<ISafeStoredEntities> GetAsync(Type type, string query) {
        var method = typeof(IRemoteEntityQueryService).GetMethod(nameof(GetAsync), BindingFlags.Instance | BindingFlags.Public)!;
        return (ISafeStoredEntities)await (dynamic)method.MakeGenericMethod(type).Invoke(this, [query])!;
    }
}
