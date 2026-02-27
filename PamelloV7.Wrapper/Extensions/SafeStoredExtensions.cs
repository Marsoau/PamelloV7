using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Extensions;

public static class SafeStoredExtensions
{
    public static Func<Type, int, object>? GetSingleAsync;
    private static Func<Type, int, object> _getSingleAsync
        => GetSingleAsync ?? throw new Exception("GetSingleAsync function is not set");
    
    public static async Task RequestAsync(this ISafeStoredEntity entity) {
        object? newEntity = await (dynamic)_getSingleAsync(entity.EntityType, entity.Id);
        if (newEntity is not IRemoteEntity remoteEntity) return;
        
        entity.Entity = remoteEntity;
    }
}
