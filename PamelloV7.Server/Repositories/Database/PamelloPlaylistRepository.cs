using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories.Database
{
    public class PamelloPlaylistRepository : PamelloDatabaseRepository<IPamelloPlaylist, DatabasePlaylist>, IPamelloPlaylistRepository
    {
        public PamelloPlaylistRepository(IServiceProvider services) : base(services) {

        }
        public override void InitServices() {
            base.InitServices();
        }

        public IPamelloPlaylist Create(string name, IPamelloUser user) {
            return user.CreatePlaylist(name);
        }

        public IPamelloPlaylist GetByNameRequired(string name)
            => GetByName(name) ?? throw new PamelloException($"Cant find playlist by name \"{name}\"");

        public IPamelloPlaylist? GetByName(string name) {
            var pamelloPlaylist = _loaded.FirstOrDefault(playlist => playlist.Name == name);
            if (pamelloPlaylist is not null) return pamelloPlaylist;

            var entities = GetEntities();

            var databasePlaylist = entities.FirstOrDefault(playlist => playlist.Name == name);
            if (databasePlaylist is null) return null;

            return Load(databasePlaylist);
        }

        public override IPamelloPlaylist? GetByValueSync(string value, IPamelloUser? scopeUser) {
            IPamelloPlaylist? playlist = null;

            if (value == "random") {
                playlist = GetRandom();
            }
            if (int.TryParse(value, out var id)) {
                playlist = Get(id);
            }

            playlist ??= GetByName(value);

            return playlist;
        }

        public IEnumerable<IPamelloPlaylist> Search(string querry, IPamelloUser? ownedBy = null, IPamelloUser? favoriteBy = null, IPamelloUser? scopeUser = null) {
            IEnumerable<IPamelloPlaylist> list = _loaded;

            if (ownedBy is not null) {
                list = list.Where(playlist => playlist.Owner.Id == ownedBy.Id);
            }
            if (favoriteBy is not null) {
                list = list.Where(playlist => playlist.FavoriteBy.Any(user => user.Id == favoriteBy.Id));
            }

            return Search(list, querry, scopeUser);
        }

        protected override IPamelloPlaylist LoadBase(DatabasePlaylist databasePlaylist) {
            var pamelloPlaylist = _loaded.FirstOrDefault(playlist => playlist.Id == databasePlaylist.Id);
            if (pamelloPlaylist is not null) return pamelloPlaylist;

            pamelloPlaylist = new PamelloPlaylist(_services, databasePlaylist);
            _loaded.Add(pamelloPlaylist);

            return pamelloPlaylist;
        }
        public override List<DatabasePlaylist> ProvideEntities() {
            return GetDatabase().Playlists
                .AsNoTracking()
                .Include(playlist => playlist.Owner)
                .Include(playlist => playlist.Entries)
                .Include(playlist => playlist.FavoriteBy)
                .AsSplitQuery()
                .ToList();
        }
        public override void Delete(IPamelloPlaylist playlist) => throw new NotImplementedException();
        
        public override void Dispose() {
            Console.WriteLine("Disposing playlists");
        }
    }
}
