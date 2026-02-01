using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data.Entities.Base;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;

namespace PamelloV7.Server.Entities.Base;

public abstract class PamelloEntity : IPamelloEntity
{
    protected readonly IServiceProvider _services;

    protected readonly IEventsService _events;
    
    protected readonly PamelloEntitySink _sink;
    
    protected readonly IPamelloUserRepository _users;
    protected readonly IPamelloSongRepository _songs;
    protected readonly IPamelloEpisodeRepository _episodes;
    protected readonly IPamelloPlaylistRepository _playlists;

    protected int _changesDepth;

    public bool IsChangesGoing => _changesDepth > 0;
    
    public int Id { get; }
    
    public abstract string Name { get; set; }

    protected PamelloEntity(int id, IServiceProvider services) {
        _services = services;
        
        _events = services.GetRequiredService<IEventsService>();
        
        _sink = new PamelloEntitySink(services, this);
        
        Id = id;
        
        _users = services.GetRequiredService<IPamelloUserRepository>();
        _songs = services.GetRequiredService<IPamelloSongRepository>();
        _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
        _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();

        _changesDepth = 0;
    }

    public void StartChanges() {
        _changesDepth++;
    }

    public virtual void EndChanges() {
        if (_changesDepth > 0) _changesDepth--;
        if (_changesDepth != 0) return;
        
        _sink.Flush();
    }

    public virtual IPamelloDTO GetDto() {
        return new PamelloEntityDTO() {
            Id = Id,
            Name = Name,
        };
    }

    public override string ToString() {
        return $"[{Id}] {Name}";
    }
}
