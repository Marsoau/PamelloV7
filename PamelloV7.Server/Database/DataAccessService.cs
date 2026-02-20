using LiteDB;
using PamelloV7.Core.Data;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Config;

namespace PamelloV7.Server.Database;

public class DatabaseAccessService : IDatabaseAccessService
{
    private readonly LiteDatabase _db;
    
    private IPamelloUserRepository _users;
    private IPamelloSongRepository _songs;
    private IPamelloEpisodeRepository _episodes;
    private IPamelloPlaylistRepository _playlists;

    public DatabaseAccessService() {
        _db = new LiteDatabase($"{ServerConfig.Root.DataPath}/lite-old70.db", GetMapper());
    }

    public void Startup(IServiceProvider services) {
        _songs = services.GetRequiredService<IPamelloSongRepository>();
        _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
        _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
        _users = services.GetRequiredService<IPamelloUserRepository>();
    }

    private BsonMapper GetMapper() {
        var mapper = new BsonMapper();
        
        mapper.RegisterType<IPamelloSong>(
            pamelloEntity => pamelloEntity.Id,
            id => _songs.GetRequired(id)
        );
        mapper.RegisterType<IPamelloPlaylist>(
            pamelloEntity => pamelloEntity.Id,
            id => _playlists.GetRequired(id)
        );
        mapper.RegisterType<IPamelloEpisode>(
            pamelloEntity => pamelloEntity.Id,
            id => _episodes.GetRequired(id)
        );
        mapper.RegisterType<IPamelloUser>(
            pamelloEntity => pamelloEntity.Id,
            id => _users.GetRequired(id)
        );
        
        mapper.RegisterType<IPamelloPlayer>(
            _ => BsonValue.Null,
            _ => null
        );
        mapper.RegisterType<IPamelloSpeaker>(
            _ => BsonValue.Null,
            _ => null
        );
        
        return mapper;
    }

    public IDatabaseCollection<TType> GetCollection<TType>(string name) {
        var collection = _db.GetCollection<TType>(name);

        return new DatabaseCollection<TType>(collection);
    }
}
