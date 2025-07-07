using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories.Database
{
    public class PamelloSongRepository : PamelloDatabaseRepository<IPamelloSong, DatabaseSong>, IPamelloSongRepository
    {
        private readonly YoutubeInfoService _youtube;
        
        private IPamelloPlaylistRepository _playlists;
        
        private YoutubeDownloadService _downloader;

        public PamelloSongRepository(IServiceProvider services,
            YoutubeInfoService youtube
        ) : base(services) {
            _youtube = youtube;
        }
        public override void InitServices() {
            base.InitServices();

            _playlists = _services.GetRequiredService<IPamelloPlaylistRepository>();
            
            _downloader = _services.GetRequiredService<YoutubeDownloadService>();
        }

        public IPamelloSong GetByNameRequired(string name)
            => GetByName(null, name) ?? throw new PamelloException($"Cant find song by name \"{name}\"");
        public IPamelloSong GetByAssociacionRequired(string associacion)
            => GetByAssociation(associacion) ?? throw new PamelloException($"Cant find song by associacion \"{associacion}\"");
        public IPamelloSong GetByYouTubeIdRequired(string youtubeId)
            => GetByYoutubeId(youtubeId) ?? throw new PamelloException($"Cant find song by youtube id \"{youtubeId}\"");

        public IPamelloSong? GetByAssociation(string ascn) {
            var db = GetDatabase();

            var databaseAssociation = db.Associations
                .Where(association => association.Association == ascn)
                .Include(dbAssociation => dbAssociation.Song)
                .FirstOrDefault();
            if (databaseAssociation is null) return null;

            return Get(databaseAssociation.Song.Id);
        }

        public IPamelloSong? GetByYoutubeId(string youtubeId) {
            var pamelloSong = _loaded.FirstOrDefault(song => song.YoutubeId == youtubeId);
            if (pamelloSong is not null) return pamelloSong;

            var entites = GetEntities();

            var databaseSong = entites.FirstOrDefault(song => song.YoutubeId == youtubeId);
            if (databaseSong is null) return null;

            return Load(databaseSong);
        }

        public override IPamelloSong? GetByValueSync(string value, IPamelloUser? scopeUser) {
            IPamelloSong? song = null;

            if (value == "current") {
                song = scopeUser?.SelectedPlayer?.Queue.Audio?.Song;
            }
            else if (value == "random") {
                song = GetRandom();
            }
            else if (int.TryParse(value, out var songId)) {
                song = Get(songId);
            }
            else if (value.StartsWith("http")) {
                var youtubeId = _youtube.GetVideoIdFromUrl(value);
                song = GetByYoutubeId(youtubeId);

                if (song is null && scopeUser is not null) {
                    song = AddAsync(youtubeId, scopeUser).Result;
                }
            }
            else if (value.Contains(':')) {
                song = GetFromSplitValue(value, scopeUser, _playlists, (playlist, secondValue) => {
                    if (!int.TryParse(secondValue, out var position))
                        return null;
                    
                    return playlist?.Songs.ElementAtOrDefault(position);
                }).Result;
            }
            else {
                song = GetByAssociation(value) ?? GetByName(null, value);
            }

            return song;
        }

        public async Task<IPamelloSong?> AddAsync(string youtubeId, IPamelloUser adder) {
            if (adder is null) return null;
            if (youtubeId?.Length != 11) return null;

            var db = GetDatabase();

            var pamelloSong = GetByYoutubeId(youtubeId);
            if (pamelloSong is not null) return pamelloSong;

            var adderUser = await db.Users.FindAsync(adder.Id);
            if (adderUser is null) return null;

			var youtubeInfo = await _youtube.GetVideoInfoAsync(youtubeId);
            if (youtubeInfo is null) return null;

			var databaseSong = new DatabaseSong {
				Name = youtubeInfo.Name,
				CoverUrl = youtubeInfo.CoverUrl,
				YoutubeId = youtubeId,
				PlayCount = 0,
                AddedAt = DateTime.UtcNow,
                AddedBy = adderUser,
                Associations = [],
				FavoriteBy = [],
				PlaylistEntries = [],
                Episodes = [],
			};

			databaseSong.Episodes = youtubeInfo.Episodes.Select(episodeInfo => new DatabaseEpisode {
				Name = episodeInfo.Name,
				Start = episodeInfo.Start,
				Song = databaseSong,
                Skip = false
			}).ToList();

			db.Songs.Add(databaseSong);
			await db.SaveChangesAsync();

            pamelloSong = Load(databaseSong);
            if (pamelloSong is null) return null;
            
            ((PamelloUser)adder)._addedSongs.Add(pamelloSong);

            _ = _downloader.DownloadFromYoutubeAsync(pamelloSong);

            return pamelloSong;
		}

        public IEnumerable<IPamelloSong> Search(string querry, IPamelloUser scopeUser, IPamelloUser? addedBy = null, IPamelloUser? favoriteBy = null) {
            IEnumerable<IPamelloSong> list = _loaded;

            if (addedBy is not null) {
                list = list.Where(song => song.AddedBy.Id == addedBy.Id);
            }
            if (favoriteBy is not null) {
                list = list.Where(song => song.FavoritedBy.Any(user => user.Id == favoriteBy.Id));
            }

            return Search(list, querry, scopeUser);
        }

        protected override IPamelloSong LoadBase(DatabaseSong databaseSong) {
            var pamelloSong = _loaded.FirstOrDefault(song => song.Id == databaseSong.Id);
            if (pamelloSong is not null) return pamelloSong;

            pamelloSong = new PamelloSong(_services, databaseSong);
            _loaded.Add(pamelloSong);

            return pamelloSong;
        }

        public override void Delete(IPamelloSong song) {
            throw new NotImplementedException();
        }

        public override List<DatabaseSong> ProvideEntities() {
            return GetDatabase().Songs
                .AsNoTracking()
                .Include(song => song.AddedBy)
                .Include(song => song.Episodes)
                .Include(song => song.PlaylistEntries)
                .Include(song => song.FavoriteBy)
                .Include(song => song.Associations)
                .AsSplitQuery()
                .ToList();
        }

        public override void Dispose() {
            Console.WriteLine("Disposing songs");
        }

        public IPamelloSong? Get(IPamelloUser scopeUser, int id) {
            return Get(id);
        }

        public IPamelloSong? GetByName(IPamelloUser scopeUser, string query) {
            var pamelloSong = _loaded.FirstOrDefault(song => song.Name == query);
            if (pamelloSong is not null) return pamelloSong;

            var entites = GetEntities();

            var databaseSong = entites.FirstOrDefault(song => song.Name == query);
            if (databaseSong is null) return null;

            return Load(databaseSong);
        }

        public IEnumerable<IPamelloSong> GetCurrent(IPamelloUser scopeUser) {
            throw new NotImplementedException();
        }

        public IEnumerable<IPamelloSong> GetRandom(IPamelloUser scopeUser) {
            throw new NotImplementedException();
        }

        public IEnumerable<IPamelloSong> GetQueue(IPamelloUser scopeUser) {
            throw new NotImplementedException();
        }

        public IEnumerable<IPamelloSong> GetAll(IPamelloUser scopeUser, IPamelloUser? addedBy = null, IPamelloUser? favoriteBy = null) {
            IEnumerable<IPamelloSong> list = _loaded;

            if (addedBy is not null) {
                list = list.Where(song => song.AddedBy.Id == addedBy.Id);
            }
            if (favoriteBy is not null) {
                list = list.Where(song => song.FavoritedBy.Any(user => user.Id == favoriteBy.Id));
            }

            return list;
        }
    }
}
