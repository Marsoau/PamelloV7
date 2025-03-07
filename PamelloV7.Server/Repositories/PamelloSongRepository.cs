using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Youtube;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories
{
    public class PamelloSongRepository : PamelloDatabaseRepository<PamelloSong, DatabaseSong>
    {
        private readonly YoutubeInfoService _youtube;

        public PamelloSongRepository(IServiceProvider services,
            YoutubeInfoService youtube
        ) : base(services) {
            _youtube = youtube;
        }
        public override void InitServices() {
            base.InitServices();
        }

        public PamelloSong GetByNameRequired(string name)
            => GetByName(name) ?? throw new PamelloException($"Cant find song by name \"{name}\"");
        public PamelloSong GetByAssociacionRequired(string associacion)
            => GetByAssociacion(associacion) ?? throw new PamelloException($"Cant find song by associacion \"{associacion}\"");
        public PamelloSong GetByYouTubeIdRequired(string youtubeId)
            => GetByYoutubeId(youtubeId) ?? throw new PamelloException($"Cant find song by youtube id \"{youtubeId}\"");

        public PamelloSong? GetByName(string name) {
            var pamelloSong = _loaded.FirstOrDefault(song => song.Name == name);
            if (pamelloSong is not null) return pamelloSong;

            var databaseSong = _nonloaded.FirstOrDefault(song => song.Name == name);
            if (databaseSong is null) return null;

            return Load(databaseSong);
        }

        public PamelloSong? GetByAssociacion(string ascn) {
            var databaseAssociacion = _database.Associacions.Find(ascn);
            if (databaseAssociacion is null) return null;

            return Get(databaseAssociacion.Song.Id);
        }

        public PamelloSong? GetByYoutubeId(string youtubeId) {
            var pamelloSong = _loaded.FirstOrDefault(song => song.YoutubeId == youtubeId);
            if (pamelloSong is not null) return pamelloSong;

            var databaseSong = _nonloaded.FirstOrDefault(song => song.YoutubeId == youtubeId);
            if (databaseSong is null) return null;

            return Load(databaseSong);
        }

        public override async Task<PamelloSong?> GetByValue(string value, PamelloUser? scopeUser = null) {
            PamelloSong? song = null;

            if (value == "current") {
                song = scopeUser?.SelectedPlayer?.Queue.Current?.Song;
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
                    song = await AddAsync(youtubeId, scopeUser);
                }
            }
            else {
                song = GetByAssociacion(value);

                if (song is null) {
                    song = GetByName(value);
                }
            }

            return song;
        }

        public PamelloSong? GetRandom() {
            if (_nonloaded.Count == 0) return null;

            var randomPosition = Random.Shared.Next(0, _nonloaded.Count);
            return Load(_nonloaded[randomPosition]);
        }
        public async Task<PamelloSong?> GetRandomPV5(PamelloUser adder) {
            DirectoryInfo pv5dir = new DirectoryInfo(@"D:\DiscordMusic");
            var pv5files = pv5dir.GetFiles();
            FileInfo file;

            do {
                file = pv5files[Random.Shared.Next(0, pv5files.Length)];
            } while (file.Extension != ".mp4");

            return await AddAsync(file.Name.Substring(0, 11), adder);
        }

        public async Task<PamelloSong?> AddAsync(string youtubeId, PamelloUser adder) {
            if (adder is null) return null;
            if (youtubeId?.Length != 11) return null;

            var pamelloSong = GetByYoutubeId(youtubeId);
            if (pamelloSong is not null) return pamelloSong;

			var youtubeInfo = await _youtube.GetVideoInfoAsync(youtubeId);
            if (youtubeInfo is null) return null;

			var databaseSong = new DatabaseSong() {
				Name = youtubeInfo.Name,
				CoverUrl = $"https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg",
				YoutubeId = youtubeId,
				PlayCount = 0,
                AddedAt = DateTime.UtcNow,
                AddedBy = adder.Entity,
                Associacions = [],
				FavoritedBy = [],
				Playlists = [],
                Episodes = [],
			};

			databaseSong.Episodes = youtubeInfo.Episodes.Select(episodeInfo => new DatabaseEpisode() {
				Name = episodeInfo.Name,
				Start = episodeInfo.Start,
				Song = databaseSong,
                Skip = false
			}).ToList();

			_database.Songs.Add(databaseSong);
			_database.SaveChanges();

            pamelloSong = Load(databaseSong);

            //event

            return pamelloSong;
		}

        public override void Delete(int id) => throw new NotImplementedException();

        public async Task<IEnumerable<PamelloSong>> Search(string querry, PamelloUser? addedBy = null, PamelloUser? favoriteBy = null, PamelloUser scopeUser = null) {
            LoadAll();

            IEnumerable<PamelloSong> list = _loaded;

            if (addedBy is not null) {
                list = list.Where(song => song.AddedBy.Id == addedBy.Id);
            }
            if (favoriteBy is not null) {
                list = list.Where(song => song.FavoritedBy.Any(user => user.Id == favoriteBy.Id));
            }

            return await Search(list, querry, scopeUser);
        }

        public override PamelloSong Load(DatabaseSong databaseSong) {
            var pamelloSong = _loaded.FirstOrDefault(song => song.Id == databaseSong.Id);
            if (pamelloSong is not null) return pamelloSong;

            pamelloSong = new PamelloSong(_services, databaseSong);
            _loaded.Add(pamelloSong);

            return pamelloSong;
        }
        public override List<DatabaseSong> LoadDatabaseEntities() {
            return _database.Songs
                .Include(song => song.Episodes)
                .Include(song => song.Playlists)
                .Include(song => song.FavoritedBy)
                .Include(song => song.Associacions)
                .ToList();
        }
    }
}
