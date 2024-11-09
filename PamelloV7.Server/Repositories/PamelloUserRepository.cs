using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories
{
    public class PamelloUserRepository : PamelloRepository<PamelloUser, DatabaseUser>
    {
        private readonly DiscordClientService _discordClients;

        public PamelloUserRepository(IServiceProvider services,
            DiscordClientService discordClients
        ) : base(services) {
            _discordClients = discordClients;
        }

        public PamelloUser? Get(Guid token) {
            var pamelloEntity = _loaded.FirstOrDefault(e => e.Token == token);
            if (pamelloEntity is not null) return pamelloEntity;

            var databaseEntity = _nonloaded.FirstOrDefault(e => e.Token == token);
            if (databaseEntity is null) return null;

            return Load(databaseEntity);
        }

        public PamelloUser? Get(ulong discordId) {
            var pamelloUser = _loaded.FirstOrDefault(e => e.DiscordUser.Id == discordId);
            if (pamelloUser is not null) return pamelloUser;

            var databaseUser = _nonloaded.FirstOrDefault(e => e.DiscordId == discordId);
            if (databaseUser is not null) return Load(databaseUser);

            var discordUser = _discordClients.MainClient.GetUser(discordId);
            if (discordUser is null) return null;

            databaseUser = new DatabaseUser() {
                DiscordId = discordId,
                Token = Guid.NewGuid(),
                SongsPlayed = 0,
                JoinedAt = DateTime.UtcNow,
                IsAdministrator = false,
                AddedSongs = [],
                OwnedPlaylists = [],
            };
            _database.Users.Add(databaseUser);
            _database.SaveChanges();

            pamelloUser = new PamelloUser(_services, databaseUser, discordUser);
            _loaded.Add(pamelloUser);

            return pamelloUser;
        }

        public override PamelloUser? Load(DatabaseUser databaseUser) {
            var discordUser = _discordClients.MainClient.GetUser(databaseUser.DiscordId);
            if (discordUser is null) return null;

            var pamelloUser = new PamelloUser(_services, databaseUser, discordUser);
            _loaded.Add(pamelloUser);

            return pamelloUser;
        }

        public override List<DatabaseUser> LoadDatabaseEntities() {
            return _database.Users
                .Include(databaseUser => databaseUser.AddedSongs)
                .Include(databaseUser => databaseUser.OwnedPlaylists)
                .ToList();
        }

        public override void Delete(int id) => throw new NotImplementedException();
    }
}
