using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Module.Marsoau.Base.Repositories.Base;

public abstract class PamelloRepository<TPamelloEntity> : IPamelloRepository<TPamelloEntity>
    where TPamelloEntity : IPamelloEntity
{
    protected readonly IServiceProvider _services;

    protected readonly List<TPamelloEntity> _loaded;

    protected IPamelloUserRepository _users;
    protected IPamelloSongRepository _songs;
    protected IPamelloEpisodeRepository _episodes;
    protected IPamelloPlaylistRepository _playlists;
    
    protected PamelloRepository(IServiceProvider services) {
        _services = services;
        
        _loaded = [];
    }

    public void StartupRepository() {
        _users = _services.GetRequiredService<IPamelloUserRepository>();
        _songs = _services.GetRequiredService<IPamelloSongRepository>();
        _episodes = _services.GetRequiredService<IPamelloEpisodeRepository>();
        _playlists = _services.GetRequiredService<IPamelloPlaylistRepository>();
    }

    public TPamelloEntity GetRequired(int id)
        => Get(id) ?? throw new PamelloException($"Entity with id {id} was not found");
    public virtual TPamelloEntity? Get(int id) {
        return _loaded.FirstOrDefault(e => e.Id == id);
    }

    public IEnumerable<TPamelloEntity> GetLoaded() {
        return _loaded;
    }

    public abstract void Delete(TPamelloEntity entity);
}
