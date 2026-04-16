using System.Diagnostics;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Extensions;

public static class SafeStoredExtensions
{
    public static Func<Type, int, object>? GetSingleAsyncFunc;
    public static Func<Type, string, object>? GetAsyncFunc;

    private static async Task<IRemoteEntity?> GetSingleAsync(Type type, int id) {
        if (GetSingleAsyncFunc is null) throw new Exception("GetSingleAsync function is not set");
        
        object? newEntity = await (dynamic)GetSingleAsyncFunc(type, id);
        if (newEntity is IRemoteEntity remoteEntity) return remoteEntity;
        
        return null;
    }
    
    private static async Task<IEnumerable<IRemoteEntity>> GetAsync(Type type, string query) {
        if (GetAsyncFunc is null) throw new Exception("GetSingleAsync function is not set");
        
        object? newEntity = await (dynamic)GetAsyncFunc(type, query);
        if (newEntity is IEnumerable<IRemoteEntity> remoteEntities) return remoteEntities;
        
        return [];
    }
    
    public static async Task<TEntityType> LoadRequiredAsync<TEntityType>(this Safe<TEntityType> entity)
        where TEntityType : class, IDeletableEntity
        => await entity.LoadAsync() ?? throw new Exception($"Failed to load entity of type {typeof(TEntityType)} with id {entity.Id}");
    public static async Task<IRemoteEntity?> LoadRequiredAsync(this ISafeStoredEntity entity)
        => await entity.LoadAsync() ?? throw new Exception($"Failed to load entity of type {entity.EntityType} with id {entity.Id}");
    
    public static async Task<TEntityType?> LoadAsync<TEntityType>(this Safe<TEntityType> entity)
        where TEntityType : class, IDeletableEntity
        => await ((ISafeStoredEntity)entity).LoadAsync() as TEntityType;
    public static async Task<IRemoteEntity?> LoadAsync(this ISafeStoredEntity entity) {
        var result = await GetSingleAsync(entity.EntityType, entity.Id);
        entity.Entity = result;
        
        return result;
    }

    public static async Task<SafeList<TEntityType>> LoadAsync<TEntityType>(this SafeList<TEntityType> entities)
        where TEntityType : class, IDeletableEntity
    {
        await ((ISafeStoredEntities)entities).LoadAsync();
        return entities;
    }
    public static async Task<IEnumerable<TEntityType>> LoadPageAsync<TEntityType>(this SafeList<TEntityType> entities, int page, int count)
        where TEntityType : class, IDeletableEntity
    {
        await ((ISafeStoredEntities)entities).LoadPageAsync(page, count);
        return entities.Skip(page * count).Take(count);
    }
    
    public static async Task LoadAsync(this ISafeStoredEntities entities) {
        await entities.LoadPageAsync(0, entities.InternalSafeEntities.Count());
    }
    public static async Task LoadPageAsync(this ISafeStoredEntities entities, int page, int count) {
        var nonloadedIds = entities.InternalSafeEntities
            .Skip(page * count).Take(count)
            .Where(entity => entity.Entity is null)
            .Select(entity => entity.Id)
            .Where(id => id != 0)
            .Distinct()
            .ToList();
        if (nonloadedIds.Count == 0) return;
        
        await GetAsync(entities.EntitiesType, string.Join(",", nonloadedIds));
    }
}
