using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Repositories.Database.Base;

namespace PamelloV7.Server.Repositories.Database;

public class PamelloSongRepository : PamelloDatabaseRepository<IPamelloSong, DatabaseSong>, IPamelloSongRepository
{
    public PamelloSongRepository(IServiceProvider services) : base(services) {
    }

    public override string CollectionName => "songs";
    protected override IPamelloSong LoadBase(DatabaseSong databaseEntity) {
        throw new NotImplementedException();
    }

    public override void Save(IPamelloSong entity) {
        throw new NotImplementedException();
    }

    public override void Delete(IPamelloSong entity) {
        throw new NotImplementedException();
    }

    public Task<IPamelloSong> AddAsync(string youtubeId, IPamelloUser adder) {
        throw new NotImplementedException();
    }

    public IPamelloSong GetRandom() {
        throw new NotImplementedException();
    }

    public IPamelloSong GetByYoutubeId(string youtubeId) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloSong> Search(string querry, IPamelloUser scopeUser, IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null) {
        throw new NotImplementedException();
    }
}
