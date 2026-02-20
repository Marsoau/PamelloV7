using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.DTO;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Entities.Base;

public abstract class PamelloDynamicEntity : IPamelloDynamicEntity
{
    protected readonly IServiceProvider _services;

    protected readonly IEventsService _events;
    
    protected readonly PamelloEntitySink _sink;
    
    protected readonly IPamelloUserRepository _users;
    protected readonly IPamelloSongRepository _songs;
    protected readonly IPamelloEpisodeRepository _episodes;
    protected readonly IPamelloPlaylistRepository _playlists;
    
    protected readonly IPamelloPlayerRepository _players;
    protected readonly IPamelloSpeakerRepository _speakers;

    protected int _changesDepth;

    public bool IsChangesGoing => _changesDepth > 0;
    
    public int Id { get; }
    
    public abstract string Name { get; protected set; }
    public abstract string SetName(string name, IPamelloUser scopeUser);

    protected PamelloDynamicEntity(int id, IServiceProvider services) {
        _services = services;
        
        _events = services.GetRequiredService<IEventsService>();
        
        _sink = new PamelloEntitySink(services, this);
        
        Id = id;
        
        _users = services.GetRequiredService<IPamelloUserRepository>();
        _songs = services.GetRequiredService<IPamelloSongRepository>();
        _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
        _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
        
        _players = services.GetRequiredService<IPamelloPlayerRepository>();
        _speakers = services.GetRequiredService<IPamelloSpeakerRepository>();

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
