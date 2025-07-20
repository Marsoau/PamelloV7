using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Entities;
using PamelloV7.Server.Repositories.Database.Base;
namespace PamelloV7.Server.Repositories.Database;

public class PamelloUserRepository : PamelloDatabaseRepository<IPamelloUser, DatabaseUser>, IPamelloUserRepository
{
    public override string CollectionName => "users";
    
    public PamelloUserRepository(IServiceProvider services) : base(services) {
    }

    protected override IPamelloUser LoadBase(DatabaseUser databaseEntity) {
        return new PamelloUser(databaseEntity, _services);
    }

    public override void Delete(IPamelloUser entity) {
        throw new NotImplementedException();
    }

    public override void Save(IPamelloUser entity) {
        throw new NotImplementedException();
    }

    public IPamelloUser? GetByToken(Guid token) {
        throw new NotImplementedException();
    }

    public IPamelloUser? GetByDiscord(ulong discordId, bool createIfNotFound = true) {
        throw new NotImplementedException();
    }
}
