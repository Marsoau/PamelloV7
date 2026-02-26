using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Framework.Containers;

public static class SafeStoredEntityStaticContainer
{
    public static Func<Type, int, IDeletableEntity?> GetById;
}

public class SafeStoredEntity<TEntityType> : ISafeStoredEntity
    where TEntityType : class, IDeletableEntity
{
    private int _id;
    private TEntityType? _entity;

    public int Id {
        get => _id;
        set {
            if (_id == value) return;
            if ((_id = value) == 0) {
                _entity = null;
                return;
            }

            _entity = GetById();
        }
    }

    public TEntityType? Entity {
        get {
            if (_id == 0) return null;
            if (_entity?.IsDeleted ?? false) _entity = null;

            return _entity ??= GetById();
        }
        set {
            if (ReferenceEquals(_entity, value)) return;
            
            _entity = value;
            _id = _entity?.Id ?? 0;
        }
    }
    public TEntityType RequiredEntity =>
        Entity ?? (_id == 0
            ? throw new PamelloException($"Required entity of type {typeof(TEntityType).Name} is null")
            : throw new PamelloException($"Required entity of type {typeof(TEntityType).Name} with id {_id} not found, most likely deleted")
        );
    
    IDeletableEntity? ISafeStoredEntity.Entity => Entity;
    IDeletableEntity ISafeStoredEntity.RequiredEntity => RequiredEntity;
    
    Type ISafeStoredEntity.EntityType => typeof(TEntityType);
    
    public SafeStoredEntity(int id) {
        Id = id;
    }

    public SafeStoredEntity(TEntityType? entity) {
        Entity = entity;
    }
    
    private TEntityType? GetById() => SafeStoredEntityStaticContainer.GetById(typeof(TEntityType), _id) as TEntityType;
    
    public override string ToString() {
        return $"<{typeof(TEntityType).Name}>({Id}:{!(Entity?.IsDeleted ?? true)})";
    }
}
