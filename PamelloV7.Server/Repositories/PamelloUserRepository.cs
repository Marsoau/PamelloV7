using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories
{
    public class PamelloUserRepository : PamelloDatabaseRepository<PamelloUser, DatabaseUser>
    {
        private readonly DiscordClientService _discordClients;

        public PamelloUserRepository(IServiceProvider services,
            DiscordClientService discordClients
        ) : base(services) {
            _discordClients = discordClients;
        }
        public override void InitServices() {
            base.InitServices();
        }

        public PamelloUser? GetByToken(Guid token) {
            var pamelloEntity = _loaded.FirstOrDefault(e => e.Token == token);
            if (pamelloEntity is not null) return pamelloEntity;

            var databaseEntity = _nonloaded.FirstOrDefault(e => e.Token == token);
            if (databaseEntity is null) return null;

            return Load(databaseEntity);
        }

        public PamelloUser? GetByDiscord(ulong discordId) {
            var pamelloUser = _loaded.FirstOrDefault(user => user.DiscordId == discordId);
            if (pamelloUser is not null) return pamelloUser;

            var databaseUser = _nonloaded.FirstOrDefault(user => user.DiscordId == discordId);
            if (databaseUser is not null) return Load(databaseUser);

            if (_discordClients.IsClientUser(discordId)) return null;

            var discordUser = _discordClients.MainClient.GetUser(discordId);

            databaseUser = new DatabaseUser() {
                DiscordId = discordId,
                Token = Guid.NewGuid(),
                SongsPlayed = 0,
                JoinedAt = DateTime.UtcNow,
                IsAdministrator = false,
                FavoriteSongs = [],
                FavoritePlaylists = [],
                OwnedPlaylists = [],
                AddedSongs = [],
            };
            _database.Users.Add(databaseUser);
            _database.SaveChanges();

            pamelloUser = new PamelloUser(_services, databaseUser, discordUser);
            _loaded.Add(pamelloUser);

            //_events.UserCreated(pamelloUser);
            //_events.UserLoaded(pamelloUser);

            return pamelloUser;
        }

        public override Task<PamelloUser?> GetByValue(string value, PamelloUser? scopeUser) {
            PamelloUser? user = null;

            if (value == "current") {
                user = scopeUser;
            }
            else if (int.TryParse(value, out var id)) {
                user = Get(id);
            }
            else if (Guid.TryParse(value, out var token)) {
                user = GetByToken(token);
            }

            return Task.FromResult(user);
        }

        public override PamelloUser? Load(DatabaseUser databaseUser) {
            var pamelloUser = _loaded.FirstOrDefault(user => user.Id == databaseUser.Id);
            if (pamelloUser is not null) return pamelloUser;

            var discordUser = _discordClients.MainClient.GetUser(databaseUser.DiscordId);
            if (discordUser is null) return null;

            pamelloUser = new PamelloUser(_services, databaseUser, discordUser);
            _loaded.Add(pamelloUser);

            //_events.UserLoaded(pamelloUser);

            return pamelloUser;
        }

        public override List<DatabaseUser> LoadDatabaseEntities() {
            return _database.Users
                .Include(databaseUser => databaseUser.FavoriteSongs)
                .Include(databaseUser => databaseUser.FavoritePlaylists)
                .Include(databaseUser => databaseUser.AddedSongs)
                .Include(databaseUser => databaseUser.OwnedPlaylists)
                .ToList();
        }

        public override void Delete(int id) => throw new NotImplementedException();
    }
}
