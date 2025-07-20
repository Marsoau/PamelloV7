using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Repositories.Database.Base;

namespace PamelloV7.Server.Repositories.Database;

public class PamelloPlaylistRepository : PamelloDatabaseRepository<IPamelloPlaylist, DatabasePlaylist>, IPamelloPlaylistRepository
{
    public PamelloPlaylistRepository(IServiceProvider services) : base(services) {
    }

    public override string CollectionName => "playlists";
    protected override IPamelloPlaylist LoadBase(DatabasePlaylist databaseEntity) {
        throw new NotImplementedException();
    }

    public override void Save(IPamelloPlaylist entity) {
        throw new NotImplementedException();
    }

    public override void Delete(IPamelloPlaylist entity) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloPlaylist> Search(string querry, IPamelloUser scopeUser, IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null) {
        throw new NotImplementedException();
    }
}
