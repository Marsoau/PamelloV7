using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Base.Entities;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;

namespace PamelloV7.Module.Marsoau.Base.Repositories.Database;

public class PamelloPlaylistRepository : PamelloDatabaseRepository<IPamelloPlaylist, DatabasePlaylist>, IPamelloPlaylistRepository
{
    public PamelloPlaylistRepository(IServiceProvider services) : base(services) {
    }

    public override string CollectionName => "playlists";
    protected override IPamelloPlaylist CreatePamelloEntity(DatabasePlaylist databaseEntity) {
        return new PamelloPlaylist(databaseEntity, _services);
    }

    public override void Delete(IPamelloPlaylist entity) {
        throw new NotImplementedException();
    }

    public IPamelloPlaylist? Get(IPamelloUser scopeUser, int id) {
        return Get(id);
    }

    public IPamelloPlaylist? GetByName(IPamelloUser scopeUser, string query) {
        if (query.Length == 0) return null;

        return _loaded.FirstOrDefault(
            playlist => playlist.Name == query
        );
    }

    public IEnumerable<IPamelloPlaylist> GetRandom(IPamelloUser scopeUser) {
        var playlist = _loaded.ElementAtOrDefault(Random.Shared.Next(_loaded.Count));
        return playlist is not null ? [playlist] : [];
    }

    public IEnumerable<IPamelloPlaylist> GetAll(IPamelloUser scopeUser, IPamelloUser? owner = null, IPamelloUser? favoriteBy = null) {
        IEnumerable<IPamelloPlaylist> results = _loaded.AsReadOnly();
        
        if (owner is not null) results = results.Where(s => s.Owner != owner);
        if (favoriteBy is not null) results = results.Where(s => s.FavoriteBy.Contains(favoriteBy));
        
        return results;
    }

    public IEnumerable<IPamelloPlaylist> GetFavorite(IPamelloUser scopeUser, IPamelloUser? by) {
        return scopeUser.FavoritePlaylists;
    }

    public IPamelloPlaylist Add(string name, IPamelloUser adder) {
        var databasePlaylist = new DatabasePlaylist() {
            Name = name,
            OwnerId = adder.Id,
            SongIds = [],
            IsProtected = false,
            AddedAt = DateTime.Now,
        };
        
        GetCollection().Add(databasePlaylist);
        
        return Load(databasePlaylist);
    }

    public IEnumerable<IPamelloPlaylist> Search(string querry, IPamelloUser scopeUser, IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null) {
        throw new NotImplementedException();
    }
}
