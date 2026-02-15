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
    
    private IPamelloSongRepository _songs;

    public DatabaseAccessService() {
        _db = new LiteDatabase($"{ServerConfig.Root.DataPath}/lite.db", GetMapper());
    }

    public void Startup(IServiceProvider services) {
        _songs = services.GetRequiredService<IPamelloSongRepository>();
    }

    private BsonMapper GetMapper() {
        var mapper = new BsonMapper();
        
        mapper.RegisterType<IPamelloSong>(pamelloEntity => {
            return pamelloEntity.Id;
        }, value => {
            return _songs.GetRequired(value);
        });
        
        return mapper;
    }

    public IDatabaseCollection<TType> GetCollection<TType>(string name) {
        var collection = _db.GetCollection<TType>(name);

        return new DatabaseCollection<TType>(collection);
    }
}
