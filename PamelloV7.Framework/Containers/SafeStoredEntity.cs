using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.Containers;

public class SafeStoredEntity<TEntityType>
    where TEntityType : class, IPamelloEntity
{
    private readonly IEntityQueryService _peql;

    private int _id;
    private TEntityType? _entity;
    
    public int Id {
        get => _id;
        set {
            if (_id == value) return;
            
            _id = value;
            _entity = _peql.GetById<TEntityType>(_id);
        }
    }

    public TEntityType? Entity {
        get {
            if (_entity?.IsDeleted ?? false) _entity = null;
            
            return _entity ??= _peql.GetById<TEntityType>(_id);
        }
        set {
            if (ReferenceEquals(_entity, value)) return;
            
            _entity = value;
            _id = _entity?.Id ?? 0;
        }
    }
    
    public SafeStoredEntity(int id, IServiceProvider services) {
        _peql = services.GetRequiredService<IEntityQueryService>();
        Id = id;
    }

    public SafeStoredEntity(TEntityType? entity, IServiceProvider services) {
        _peql = services.GetRequiredService<IEntityQueryService>();
        Entity = entity;
    }
}
