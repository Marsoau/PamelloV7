using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Data.Entities.Base;
using PamelloV7.Framework.Dto;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;

namespace PamelloV7.Module.Marsoau.Database.Entities.Base;

public abstract class PamelloDatabaseEntity<TDatabaseEntity> : PamelloDynamicEntity
    where TDatabaseEntity : DatabaseEntity
{
    private TDatabaseEntity? _databaseEntity;
    protected TDatabaseEntity DatabaseEntity => _databaseEntity ?? throw new InvalidOperationException("Database entity is already un bound / not bound");
    
    protected PamelloDatabaseEntity(TDatabaseEntity databaseEntity, IServiceProvider services) : base(databaseEntity.Id, services) {
        _databaseEntity = databaseEntity;
    }

    protected abstract void InitBase();
    public void Init() {
        if (_databaseEntity is null) return;
        
        InitBase();
        
        _databaseEntity = null;
    }

    public abstract void SaveInternal();
    public void Save() {
        if (!IsChangesGoing) SaveInternal();
    }

    public override void EndChanges() {
        base.EndChanges();
        Save();
    }
}
