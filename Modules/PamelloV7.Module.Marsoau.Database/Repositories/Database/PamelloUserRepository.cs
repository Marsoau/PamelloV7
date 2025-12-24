using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Base.Entities;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;
using PamelloV7.Server.Entities;

namespace PamelloV7.Module.Marsoau.Base.Repositories.Database;

public class PamelloUserRepository : PamelloDatabaseRepository<IPamelloUser, DatabaseUser>, IPamelloUserRepository
{
    public override string CollectionName => "users";
    
    public PamelloUserRepository(IServiceProvider services) : base(services) {
    }

    protected override IPamelloUser CreatePamelloEntity(DatabaseUser databaseEntity) {
        return new PamelloUser(databaseEntity, _services);
    }

    public IPamelloUser? Get(IPamelloUser scopeUser, int id) {
        return Get(id);
    }

    public IPamelloUser? GetByName(IPamelloUser scopeUser, string query) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloUser> GetAll(IPamelloUser scopeUser) {
        return _loaded.AsReadOnly();
    }

    public IEnumerable<IPamelloUser> GetCurrent(IPamelloUser scopeUser) {
        return [scopeUser];
    }

    public IEnumerable<IPamelloUser> GetRandom(IPamelloUser scopeUser) {
        var user = _loaded.ElementAtOrDefault(Random.Shared.Next(_loaded.Count));
        return user is not null ? [user] : [];
    }

    public IEnumerable<IPamelloUser> GetByToken(IPamelloUser scopeUser, Guid token) {
        var user = GetByToken(token);
        return user is not null ? [user] : [];
    }

    public IPamelloUser? GetByToken(Guid token) {
        return _loaded.FirstOrDefault(u => u.Token == token);
    }

    public IPamelloUser? GetByDiscord(ulong discordId, bool createIfNotFound = true) {
        throw new NotImplementedException();
    }

    public IPamelloUser Add() {
        var databaseUser = new DatabaseUser() {
            Token = Guid.NewGuid(),
            FavoritePlaylistIds = [],
            FavoriteSongIds = [],
            Authorizations = [],
            JoinedAt = DateTime.Now,
        };
        
        GetCollection().Add(databaseUser);

        return Load(databaseUser);
    }

    public override void Delete(IPamelloUser entity) {
        throw new NotImplementedException();
    }
}
