using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data.Entities.Base;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Repositories;

namespace PamelloV7.Server.Entities.Base;

public abstract class PamelloEntity<TDatabaseEntity> : IPamelloEntity
    where TDatabaseEntity : DatabaseEntity
{
    protected readonly IServiceProvider _services;
    
    protected readonly IPamelloUserRepository _users;
    protected readonly IPamelloSongRepository _songs;
    protected readonly IPamelloEpisodeRepository _episodes;
    protected readonly IPamelloPlaylistRepository _playlists;
    
    protected TDatabaseEntity _databaseEntity { get; private set; }
    
    public int Id { get; }
    
    public abstract string Name { get; set; }

    protected PamelloEntity(TDatabaseEntity databaseEntity, IServiceProvider services) {
        _services = services;
        
        _databaseEntity = databaseEntity;
        Id = databaseEntity.Id;
        
        _users = services.GetRequiredService<IPamelloUserRepository>();
        _songs = services.GetRequiredService<IPamelloSongRepository>();
        _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
        _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
    }
    
    public virtual IPamelloDTO GetDto() {
        return new PamelloEntityDTO() {
            Id = Id,
            Name = Name,
        };
    }

    protected abstract void InitBase();
    public void Init() {
        if (_databaseEntity is null) return;
        
        InitBase();
        
        _databaseEntity = null;
    }

    public abstract void Save();

    public override string ToString() {
        return $"[{Id}] {Name}";
    }
}
