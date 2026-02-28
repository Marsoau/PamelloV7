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
    
    public static async Task LoadAsync(this ISafeStoredEntity entity) {
        entity.Entity = await GetSingleAsync(entity.EntityType, entity.Id);
    }

    public static async Task<SafeStoredEntities<TEntityType>> LoadAsync<TEntityType>(this SafeStoredEntities<TEntityType> entities)
        where TEntityType : class, IDeletableEntity
    {
        await ((ISafeStoredEntities)entities).LoadAsync();
        
        return entities;
    }
    public static async Task LoadAsync(this ISafeStoredEntities entities) {
        await entities.LoadPageAsync(0, entities.InternalSafeEntities.Count());
    }
    public static async Task LoadPageAsync(this ISafeStoredEntities entities, int offset, int count) {
        var nonloadedIds = entities.InternalSafeEntities
            .Skip(offset).Take(count)
            .Where(entity => entity.Entity is null)
            .Select(entity => entity.Id)
            .Distinct()
            .ToList();
        if (nonloadedIds.Count == 0) return;
        
        await GetAsync(entities.EntitiesType, string.Join(",", nonloadedIds));
    }
}
