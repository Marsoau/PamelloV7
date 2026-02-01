using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data;
using PamelloV7.Core.Data.Entities.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Module.Marsoau.Base.Repositories.Base;

namespace PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;

public abstract class PamelloDatabaseRepository<TPamelloEntity, TDatabaseEntity> : PamelloRepository<TPamelloEntity>, IPamelloDatabaseRepository<TPamelloEntity>
    where TPamelloEntity : IPamelloDatabaseEntity
    where TDatabaseEntity : DatabaseEntity
{
    private readonly IDatabaseAccessService _database;

    public abstract string CollectionName { get; }
    
    public event Action? OnLoadingStart;
    public event Action<int, int>? OnLoadingProgress;
    public event Action? OnLoadingEnd;
    
    public event Action? OnInitStart;
    public event Action<int, int>? OnInitProgress;
    public event Action? OnInitEnd;
    
    public PamelloDatabaseRepository(IServiceProvider services) : base(services) {
        _database = services.GetRequiredService<IDatabaseAccessService>();
    }

    public IDatabaseCollection<TDatabaseEntity> GetCollection() {
        return _database.GetCollection<TDatabaseEntity>(CollectionName);
    }

    public void LoadAll() {
        StartupRepository();
        
        OnLoadingStart?.Invoke();

        var collection = GetCollection();
        var entities = collection.GetAll().ToArray();

        var count = 0;
        var total = entities.Length;

        foreach (var databaseEntity in entities) {
            Load(databaseEntity, false);
            
            OnLoadingProgress?.Invoke(++count, total);
        }

        OnLoadingEnd?.Invoke();
    }
    public void InitAll() {
        OnInitStart?.Invoke();

        var count = 0;
        var total = _loaded.Count;

        foreach (var entity in _loaded) {
            entity.Init();
            
            OnInitProgress?.Invoke(++count, total);
        }

        OnInitEnd?.Invoke();
    }
    
    public async Task LoadAllAsync()
        => await Task.Run(LoadAll);
    public async Task InitAllAsync()
        => await Task.Run(InitAll);

    protected abstract TPamelloEntity CreatePamelloEntity(TDatabaseEntity databaseEntity);
    
    public TPamelloEntity Load(TDatabaseEntity databaseEntity, bool initialize = true) {
        var entity = _loaded.FirstOrDefault(e => e.Id == databaseEntity.Id);
        if (entity is not null) return entity;
        
        entity = CreatePamelloEntity(databaseEntity);
        
        if (initialize) entity.Init();
        
        _loaded.Add(entity);
        
        return entity;
    }

    public override TPamelloEntity? Get(int id) {
        var entity = base.Get(id);
        if (entity is not null) return entity;
        
        var databaseEntity = GetCollection().Get(id);
        if (databaseEntity is null) return default;
        
        return Load(databaseEntity);
    }
}
