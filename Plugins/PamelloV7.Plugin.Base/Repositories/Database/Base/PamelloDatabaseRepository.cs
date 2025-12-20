using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data;
using PamelloV7.Core.Data.Entities.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Plugin.Base.Repositories.Database.Base;

public abstract class PamelloDatabaseRepository<TPamelloEntity, TDatabaseEntity> : IPamelloDatabaseRepository<TPamelloEntity>
    where TPamelloEntity : IPamelloDatabaseEntity
    where TDatabaseEntity : DatabaseEntity
{
    protected readonly IServiceProvider _services;
    
    private readonly IDatabaseAccessService _database;

    protected readonly List<TPamelloEntity> _loaded;

    public abstract string CollectionName { get; }
    
    public event Action? OnLoadingStart;
    public event Action<int, int>? OnLoadingProgress;
    public event Action? OnLoadingEnd;
    
    public event Action? OnInitStart;
    public event Action<int, int>? OnInitProgress;
    public event Action? OnInitEnd;

    protected IPamelloUserRepository _users;
    protected IPamelloSongRepository _songs;
    protected IPamelloEpisodeRepository _episodes;
    protected IPamelloPlaylistRepository _playlists;
    
    public PamelloDatabaseRepository(IServiceProvider services) {
        _services = services;
        
        _database = services.GetRequiredService<IDatabaseAccessService>();
        
        _loaded = [];
    }

    public void StartupRepository() {
        _users = _services.GetRequiredService<IPamelloUserRepository>();
        _songs = _services.GetRequiredService<IPamelloSongRepository>();
        _episodes = _services.GetRequiredService<IPamelloEpisodeRepository>();
        _playlists = _services.GetRequiredService<IPamelloPlaylistRepository>();
    }

    public IDatabaseCollection<TDatabaseEntity> GetCollection() {
        return _database.GetCollection<TDatabaseEntity>(CollectionName);
    }

    public void LoadAll() {
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

    public TPamelloEntity GetRequired(int id)
        => Get(id) ?? throw new PamelloException($"Entity with id {id} was not found");
    public TPamelloEntity? Get(int id) {
        var entity = _loaded.FirstOrDefault(e => e.Id == id);
        if (entity is not null) return entity;
        
        var databaseEntity = GetCollection().Get(id);
        if (databaseEntity is null) return default;
        
        return Load(databaseEntity);
    }

    public async Task<TPamelloEntity> GetByValueRequired(string value, IPamelloUser? scopeUser)
        => await GetByValue(value, scopeUser) ?? throw new PamelloException($"Entity with value {value} was not found");
    public Task<TPamelloEntity?> GetByValue(string value, IPamelloUser? scopeUser) {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TPamelloEntity>> SearchAsync(string query, IPamelloUser? scopeUser = null) {
        throw new NotImplementedException();
    }

    public IEnumerable<TPamelloEntity> GetLoaded() {
        return _loaded;
    }

    public abstract void Delete(TPamelloEntity entity);
}
