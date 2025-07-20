using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Entities;
using PamelloV7.Server.Repositories.Database.Base;

namespace PamelloV7.Server.Repositories.Database;

public class PamelloEpisodeRepository : PamelloDatabaseRepository<IPamelloEpisode, DatabaseEpisode>, IPamelloEpisodeRepository
{
    public override string CollectionName => "episodes";
    
    public PamelloEpisodeRepository(IServiceProvider services) : base(services) {
    }

    protected override IPamelloEpisode LoadBase(DatabaseEpisode databaseEntity) {
        return new PamelloEpisode(databaseEntity, _services);
    }

    public override void Save(IPamelloEpisode entity) {
        throw new NotImplementedException();
    }

    public override void Delete(IPamelloEpisode entity) {
        throw new NotImplementedException();
    }

    public void DeleteAllFrom(IPamelloSong song) {
        throw new NotImplementedException();
    }
}
