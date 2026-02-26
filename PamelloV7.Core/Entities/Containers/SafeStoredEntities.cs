using System.Collections;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Framework.Containers;

public class SafeStoredEntities<TEntityType> : ISafeStoredEntities, IEnumerable<TEntityType>
    where TEntityType : class, IDeletableEntity
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
    
    IEnumerable<IDeletableEntity?> ISafeStoredEntities.InternalEntities {
        get => InternalEntities;
        set => InternalEntities = value.OfType<TEntityType>();
    }
    IEnumerable<IDeletableEntity> ISafeStoredEntities.Entities =>
        _safeEntities.Select(x => x.Entity).OfType<IDeletableEntity>();

    Type ISafeStoredEntities.EntitiesType => typeof(TEntityType);

    public SafeStoredEntities() {
        _safeEntities = [];
    }
    public SafeStoredEntities(IEnumerable<int> ids) {
        InternalIds = ids;
    }
    public SafeStoredEntities(IEnumerable<TEntityType?> entities) {
        InternalEntities = entities;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<TEntityType> GetEnumerator()
        => _safeEntities.Select(x => x.Entity).OfType<TEntityType>().GetEnumerator();

    public override string ToString() {
        return $"<{typeof(TEntityType).Name}>[{string.Join(", ", InternalIds)}]";
    }
}
