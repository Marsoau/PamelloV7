using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.Containers;

public class SafeStoredEntities<TEntityType> : IEnumerable<TEntityType>
    where TEntityType : class, IPamelloEntity
{
    private List<SafeStoredEntity<TEntityType>> _safeEntities;

    public IEnumerable<int> InternalIds {
        get => _safeEntities.Select(safeEntity => safeEntity.Id);
        set => _safeEntities = value.Select(id => new SafeStoredEntity<TEntityType>(id)).ToList();
        
    }
    public IEnumerable<TEntityType?> InternalEntities {
        get => _safeEntities.Select(safeEntity => safeEntity.Entity);
        set => _safeEntities = value.Select(entity => new SafeStoredEntity<TEntityType>(entity)).ToList();
    }
    
    public SafeStoredEntities() {
        _safeEntities = [];
    }
    public SafeStoredEntities(IEnumerable<int> ids) {
        InternalIds = ids;
    }
    public SafeStoredEntities(IEnumerable<TEntityType?> entities) {
        InternalEntities = entities;
    }

    public IEnumerator<TEntityType> GetEnumerator() {
        return _safeEntities.Select(x => x.Entity).OfType<TEntityType>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
