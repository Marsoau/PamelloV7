using System.Collections;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Framework.Containers;

public class SafeList<TEntityType> : ISafeStoredEntities, IEnumerable<TEntityType>
    where TEntityType : class, IDeletableEntity
{
    private List<Safe<TEntityType>> _safeEntities;

    public IEnumerable<int> InternalIds {
        get => _safeEntities.Select(safeEntity => safeEntity.Id);
        set => _safeEntities = value.Select(id => new Safe<TEntityType>(id)).ToList();
        
    }

    public IEnumerable<TEntityType?> InternalEntities {
        get => _safeEntities.Select(safeEntity => safeEntity.Entity);
        set => _safeEntities = value.Select(entity => new Safe<TEntityType>(entity)).ToList();
    }

    public IEnumerable<Safe<TEntityType>> InternalSafeEntities => _safeEntities;

    IEnumerable<IDeletableEntity?> ISafeStoredEntities.InternalEntities {
        get => InternalEntities;
        set => InternalEntities = value.OfType<TEntityType>();
    }
    IEnumerable<IDeletableEntity> ISafeStoredEntities.Entities =>
        _safeEntities.Select(x => x.Entity).OfType<IDeletableEntity>();
    IEnumerable<ISafeStoredEntity> ISafeStoredEntities.InternalSafeEntities
        => _safeEntities;

    Type ISafeStoredEntities.EntitiesType => typeof(TEntityType);

    public SafeList() {
        _safeEntities = [];
    }
    public SafeList(IEnumerable<int> ids) {
        _safeEntities = null!;
        InternalIds = ids;
    }
    public SafeList(IEnumerable<TEntityType?> entities) {
        _safeEntities = null!;
        InternalEntities = entities;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<TEntityType> GetEnumerator()
        => _safeEntities.Select(x => x.Entity).OfType<TEntityType>().GetEnumerator();

    public override string ToString() {
        return $"<{typeof(TEntityType).Name}>[{string.Join(", ", InternalIds)}]";
    }
}
