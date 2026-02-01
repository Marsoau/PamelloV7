using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data.Entities.Base;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Server.Entities.Base;

namespace PamelloV7.Module.Marsoau.Database.Entities.Base;

public abstract class PamelloDatabaseEntity<TDatabaseEntity> : PamelloEntity
    where TDatabaseEntity : DatabaseEntity
{
    protected TDatabaseEntity _databaseEntity { get; private set; }
    
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
