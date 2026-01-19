using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Base.Entities;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;
using PamelloV7.Server.Entities;

namespace PamelloV7.Module.Marsoau.Base.Repositories.Database;

public class PamelloUserRepository : PamelloDatabaseRepository<IPamelloUser, DatabaseUser>, IPamelloUserRepository
{
    private readonly IPlatformService _platforms;
    
    public override string CollectionName => "users";
    
    public PamelloUserRepository(IServiceProvider services) : base(services) {
        _platforms = services.GetRequiredService<IPlatformService>();
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

    public IPamelloUser? GetByPlatformKey(IPamelloUser scopeUser, PlatformKey pk, bool allowCreation = false) {
        var user = _loaded.FirstOrDefault(s => s.Authorizations.Any(authorization => authorization.PK == pk));
        Console.WriteLine($"User by pk {(user is not null ? $"found: {user}" : "not found")}");
        if (user is not null) return user;
        
        if (!allowCreation) return null;
        
        var platform = _platforms.GetUserPlatform(pk.Platform);
        if (platform is null) return null;
        
        var userInfo = platform.GetUserInfo(pk.Key);
        Console.WriteLine($"Info by pk {(userInfo is not null ? $"found: {userInfo}" : "not found")}");
        if (userInfo is null) return null;
        
        Console.WriteLine("Adding user by info");
        return Add(userInfo);
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

    public IPamelloUser Add(IUserInfo info) {
        var databaseUser = new DatabaseUser() {
            Token = Guid.NewGuid(),
            FavoritePlaylistIds = [],
            FavoriteSongIds = [],
            SelectedAuthorization = 0,
            Authorizations = [
                new PlatformKey(info.Platform.Name, info.Key)
            ],
            JoinedAt = DateTime.Now,
        };
        
        GetCollection().Add(databaseUser);

        return Load(databaseUser);
    }

    public override void Delete(IPamelloUser entity) {
        throw new NotImplementedException();
    }
}
