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

        }

        public PamelloSong GetByNameRequired(string name)
            => GetByName(name) ?? throw new PamelloException($"Cant find song by name \"{name}\"");
        public PamelloSong GetByAssociacionRequired(string associacion)
            => GetByAssociacion(associacion) ?? throw new PamelloException($"Cant find song by associacion \"{associacion}\"");
        public PamelloSong GetByYouTubeIdRequired(string youtubeId)
            => GetByYoutubeId(youtubeId) ?? throw new PamelloException($"Cant find song by youtube id \"{youtubeId}\"");
        public async Task<PamelloSong> GetByValueRequired(string value)
            => await GetByValue(value) ?? throw new PamelloException($"Cant find song by value \"{value}\"");

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

        public async Task<PamelloSong?> GetByValue(string value, PamelloUser? adder = null) {
            PamelloSong? song = null;

            if (int.TryParse(value, out var songId)) {
                song = Get(songId);
            }
            else if (value.StartsWith("http")) {
                var youtubeId = _youtube.GetVideoIdFromUrl(value);
                song = GetByYoutubeId(youtubeId);

                if (song is null && adder is not null) {
                    song = await AddAsync(youtubeId, adder);
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

            return await AddAsync(file.Name, adder);
        }

        public async Task<PamelloSong?> AddAsync(string youtubeId, PamelloUser adder) {
            if (adder is null) return null;

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

        public List<PamelloSong> Search(string querry, PamelloUser? addedBy = null, PamelloUser? favoriteBy = null) {
            LoadAll();

            IEnumerable<PamelloSong> list = _loaded;

            if (addedBy is not null) {
                list = list.Where(song => song.AddedBy.Id == addedBy.Id);
            }
            if (favoriteBy is not null) {
                list = list.Where(song => song.FavoritedBy.Any(user => user.Id == favoriteBy.Id));
            }

            return Search(list, querry);
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
