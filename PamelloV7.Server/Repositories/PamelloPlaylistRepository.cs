﻿using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.Exceptions;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public class PamelloPlaylistRepository : PamelloDatabaseRepository<PamelloPlaylist, DatabasePlaylist>
    {
        public PamelloPlaylistRepository(IServiceProvider services) : base(services) {

        }
        public override void InitServices() {
            base.InitServices();
        }

        public PamelloPlaylist Create(string name, PamelloUser user) {
            return user.CreatePlaylist(name);
        }

        public PamelloPlaylist GetByNameRequired(string name)
            => GetByName(name) ?? throw new PamelloException($"Cant find playlist by name \"{name}\"");

        public PamelloPlaylist? GetByName(string name) {
            var pamelloPlaylist = _loaded.FirstOrDefault(playlist => playlist.Name == name);
            if (pamelloPlaylist is not null) return pamelloPlaylist;

            var entities = GetEntities();

            var databasePlaylist = entities.FirstOrDefault(playlist => playlist.Name == name);
            if (databasePlaylist is null) return null;

            return Load(databasePlaylist);
        }

        public override Task<PamelloPlaylist?> GetByValue(string value, PamelloUser? scopeUser = null) {
            PamelloPlaylist? playlist = null;

            if (int.TryParse(value, out int id)) {
                playlist = Get(id);
            }

            playlist ??= GetByName(value);

            return Task.FromResult(playlist);
        }

        public async Task<IEnumerable<PamelloPlaylist>> Search(string querry, PamelloUser? ownedBy = null, PamelloUser? favoriteBy = null, PamelloUser? scopeUser = null) {
            IEnumerable<PamelloPlaylist> list = _loaded;

            if (ownedBy is not null) {
                list = list.Where(playlist => playlist.OwnedBy.Id == ownedBy.Id);
            }
            if (favoriteBy is not null) {
                list = list.Where(playlist => playlist.FavoritedBy.Any(user => user.Id == favoriteBy.Id));
            }

            return await Search(list, querry, scopeUser);
        }

        protected override PamelloPlaylist LoadBase(DatabasePlaylist databasePlaylist) {
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
                .Include(playlist => playlist.Songs)
                .Include(playlist => playlist.FavoritedBy)
                .AsSplitQuery()
                .ToList();
        }
        public override void Delete(PamelloPlaylist playlist) => throw new NotImplementedException();
    }
}
