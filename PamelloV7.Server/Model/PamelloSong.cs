using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Services;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;
using PamelloV7.DAL;
using Microsoft.EntityFrameworkCore;
using PamelloV7.Core;

namespace PamelloV7.Server.Model
{
    public class PamelloSong : PamelloEntity<DatabaseSong>
    {
        private readonly YoutubeDownloadService _downloader;

        private string _name;
        private string _youtubeId;
        private string _coverUrl;
        private int _playCount;
        private DateTime _addedAt;
        private PamelloUser _addedBy;

        public override string Name {
            get => _name;
            set {
                if (_name == value) return;

                _name = value;
                Save();

                _events.Broadcast(new SongNameUpdated() {
                    SongId = Id,
                    Name = Name,
                });
            }
        }
        public string YoutubeId {
            get => _youtubeId;
        }
        public string CoverUrl {
            get => _coverUrl;
            set {
                _coverUrl = value;
                Save();

                _events.Broadcast(new SongCoverUrlUpdated() {
                    SongId = Id,
                    CoverUrl = CoverUrl
                });
            }
        }
        public int PlayCount {
            get => _playCount;
            set {
                if (_playCount == value) return;

                _playCount = value;
                Save();

                _events.Broadcast(new SongPlayCountUpdated() {
                    SongId = Id,
                    PlayCount = PlayCount
                });
            }
        }
        public DateTime AddedAt {
            get => _addedAt;
        }

        public bool IsDownloaded {
            get {
                var file = new FileInfo($@"{AppContext.BaseDirectory}Data/Music/{Id}.opus");

                if (!file.Exists) return false;
                if (_downloader.IsDownloading(this)) return false;
                if (file.Length == 0) return false;

                return true;
            }
        }

        public PamelloUser AddedBy {
            get => _addedBy;
        }

        private List<PamelloUser> _favoritedBy;
        private List<PamelloEpisode> _episodes;
        private List<PamelloPlaylist> _playlists;
        private List<string> _associacions;

        public IReadOnlyList<PamelloUser> FavoritedBy {
            get => _favoritedBy;
        }

        public IReadOnlyList<PamelloEpisode> Episodes {
            get => _episodes;
        }

        public IReadOnlyList<PamelloPlaylist> Playlists {
            get => _playlists;
        }

        public IReadOnlyList<string> Associacions {
            get => _associacions;
        }

        public IEnumerable<int> FavoriteByIds {
            get => _favoritedBy.Select(e => e.Id);
        }
        public IEnumerable<int> EpisodesIds {
            get => _favoritedBy.Select(e => e.Id);
            /*
            get {
                var list = Entity.Episodes.ToList();
                list.Sort((a, b) => {
                    if (a == null && b == null) return 0;
                    if (a == null) return -1;
                    if (b == null) return 1;

                    if (a.Start > b.Start) return 1;
                    if (a.Start < b.Start) return -1;
                    return 0;
                });
                return list.Select(databaseEpisode => databaseEpisode.Id);
            }
            */
        }
        public IEnumerable<int> PlaylistsIds {
            get => _playlists.Select(e => e.Id);
        }

        public PamelloSong(IServiceProvider services,
            DatabaseSong databaseSong
        ) : base(databaseSong, services) {
            _downloader = services.GetRequiredService<YoutubeDownloadService>();

            _name = databaseSong.Name;
            _youtubeId = databaseSong.YoutubeId;
            _coverUrl = databaseSong.CoverUrl;
            _playCount = databaseSong.PlayCount;
            _addedAt = databaseSong.AddedAt;
        }

        protected override void InitSet() {
            if (DatabaseEntity is null) return;

            _addedBy = _users.GetRequired(DatabaseEntity.AddedBy.Id);

            _favoritedBy = DatabaseEntity.FavoritedBy.Select(e => _users.Get(e.Id)).OfType<PamelloUser>().ToList();
            _episodes = DatabaseEntity.Episodes.Select(e => base._episodes.Get(e.Id)).OfType<PamelloEpisode>().ToList();
            _playlists = DatabaseEntity.FavoritedBy.Select(e => base._playlists.Get(e.Id)).OfType<PamelloPlaylist>().ToList();
            _associacions = DatabaseEntity.Associacions.Where(e => e.Song.Id == Id).Select(e => e.Associacion).ToList();
        }

        public void AddAssociacion(string associacion) {
            if (DatabaseAssociacion.Reserved.Contains(associacion)) {
                throw new PamelloException($"Associacion \"{associacion}\" is reserved and cannot be assigned to a song");
            }

            var db = GetDatabase();

            var databaseAssociacion = db.Associacions.Find(associacion);
            if (databaseAssociacion is not null) {
                if (databaseAssociacion.Song.Id == Id)
                    throw new PamelloException($"Associacion \"{associacion}\" already exist this song");

                throw new PamelloException($"Associacion \"{associacion}\" already exist for another song");
            }

            databaseAssociacion = new DatabaseAssociacion() {
                Associacion = associacion,
                Song = db.Songs.Find(Id)
            };

            db.Associacions.Add(databaseAssociacion);
            db.SaveChanges();

            _associacions.Add(associacion);

            _events.Broadcast(new SongAssociacionsUpdated() {
                SongId = Id,
                Associacions = Associacions
            });
        }

        public void RemoveAssociacion(string associacion) {
            var db = GetDatabase();

            if (!_associacions.Contains(associacion)) {
                throw new PamelloException("This song doesnt contain that associacion");
            }

            var databaseAssociacion = db.Associacions.Find(associacion);

            if (databaseAssociacion is not null) {
                db.Associacions.Remove(databaseAssociacion);
                db.SaveChanges();
            }

            _associacions.Remove(associacion);

            _events.Broadcast(new SongAssociacionsUpdated() {
                SongId = Id,
                Associacions = Associacions
            });
        }

        public PamelloEpisode AddEpisode(AudioTime start, string name, bool autoSkip) {
            var db = GetDatabase();

            var dbSong = db.Songs.Find(Id);
            if (dbSong is null) throw new PamelloException("Song doesnt exist in the database for some reason, cant add episode");

            var databaseEpisode = new DatabaseEpisode() {
                Name = name,
                Start = start.TotalSeconds,
                Skip = autoSkip,
                Song = dbSong,
            };

            db.Episodes.Add(databaseEpisode);
            db.SaveChanges();

            var episode = base._episodes.Load(databaseEpisode);
            episode.Init();

            _episodes.Add(episode);

            _events.Broadcast(new EpisodeCreated() { 
                EpisodeId = databaseEpisode.Id,
            });
            _events.Broadcast(new SongEpisodesIdsUpdated() { 
                SongId = Id,
                EpisodesIds = EpisodesIds,
            });

            return episode;
        }
        public void RemoveEpisodeAt(int position) {
            var episode = Episodes.ElementAtOrDefault(position);
            if (episode is null) throw new PamelloException($"Episode in position {position} was not found");

            RemoveEpisode(episode);
        }
        public void RemoveEpisode(PamelloEpisode episode) {
            if (!_episodes.Remove(episode)) return;

            base._episodes.Delete(episode);

            _events.Broadcast(new SongEpisodesIdsUpdated() {
                SongId = Id,
                EpisodesIds = EpisodesIds,
            });
        }
        public void RemoveAllEpisodes() {
            var oldEpisodes = _episodes;

            _episodes.Clear();

            foreach (var episode in oldEpisodes) {
                base._episodes.Delete(episode);
            }

            _events.Broadcast(new SongEpisodesIdsUpdated() { 
                SongId = Id,
                EpisodesIds = EpisodesIds,
            });
        }

        public void MakeFavorited(PamelloUser user) {
            if (_favoritedBy.Contains(user)) return;

            _favoritedBy.Add(user);
            Save();

            user.AddFavoriteSong(this);
            _events.Broadcast(new SongFavoriteByIdsUpdated() {
                SongId = Id,
                FavoriteByIds = FavoriteByIds
            });
        }
        public void UnmakeFavorited(PamelloUser user) {
            if (!_favoritedBy.Contains(user)) return;

            _favoritedBy.Remove(user);
            Save();

            user.RemoveFavoriteSong(this);
            _events.Broadcast(new SongFavoriteByIdsUpdated() {
                SongId = Id,
                FavoriteByIds = FavoriteByIds
            });
        }

        public void AddToPlaylist(PamelloPlaylist playlist) {
            if (_playlists.Contains(playlist)) return;

            _playlists.Add(playlist);
            Save();

            playlist.AddSong(this);
            _events.Broadcast(new SongPlaylistsIdsUpdated() {
                SongId = Id,
                PlaylistsIds = PlaylistsIds,
            });
        }
        public void RemoveFromPlaylist(PamelloPlaylist playlist) {
            if (_playlists.Contains(playlist)) return;

            _playlists.Remove(playlist);
            Save();

            playlist.RemoveSong(this);
            _events.Broadcast(new SongPlaylistsIdsUpdated() {
                SongId = Id,
                PlaylistsIds = PlaylistsIds,
            });
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloSongDTO {
                Id = Id,
                Name = Name,
                YoutubeId = YoutubeId,
                CoverUrl = CoverUrl,
                PlayCount = PlayCount,
                AddedById = AddedBy.Id,
                AddedAt = AddedAt,

                Associacions = Associacions,
                FavoriteByIds = FavoriteByIds,
                PlaylistsIds = PlaylistsIds,
                EpisodesIds = EpisodesIds,

                IsDownloading = _downloader.IsDownloading(this),
                DownloadProgress = _downloader.GetProgress(this),
            };
        }

        public override DatabaseSong GetDatabaseEntity(DatabaseContext? db = null) {
            db ??= GetDatabase();

            var dbSong = db.Songs
                .Where(databaseSong => databaseSong.Id == Id)
                .Include(databaseSong => databaseSong.AddedBy)
                .Include(databaseSong => databaseSong.Playlists)
                .Include(databaseSong => databaseSong.FavoritedBy)
                .Include(databaseSong => databaseSong.Episodes)
                .Include(databaseSong => databaseSong.Associacions)
                .AsSplitQuery()
                .FirstOrDefault();
            if (dbSong is null) throw new PamelloDatabaseSaveException();

            dbSong.Name = Name;
            dbSong.YoutubeId = YoutubeId;
            dbSong.CoverUrl = CoverUrl;
            dbSong.PlayCount = PlayCount;
            dbSong.AddedAt = AddedAt;

            var dbPlaylistsIds = dbSong.Playlists.Select(playlist => playlist.Id);
            var dbFavoriteByIds = dbSong.FavoritedBy.Select(user => user.Id);
            var dbEpisodesIds = dbSong.Episodes.Select(episode => episode.Id);
            var dbAssociacionsValues = dbSong.Associacions.Select(associacion => associacion.Associacion);

            var playlistsDifference = DifferenceResult<int>.From(
                dbPlaylistsIds, 
                PlaylistsIds,
                true
            );
            var favoriteByDifference = DifferenceResult<int>.From(
                dbFavoriteByIds, 
                FavoriteByIds,
                true
            );
            var episodesDifference = DifferenceResult<int>.From(
                dbEpisodesIds, 
                EpisodesIds,
                true
            );
            var associacionsDifference = DifferenceResult<string>.From(
                dbAssociacionsValues, 
                Associacions,
                true
            );

            playlistsDifference.ExcludeMoved();
            favoriteByDifference.ExcludeMoved();
            episodesDifference.ExcludeMoved();
            associacionsDifference.ExcludeMoved();

            playlistsDifference.Apply(dbSong.Playlists, (id) => {
                return db.Playlists.Find(id)!;
            });
            favoriteByDifference.Apply(dbSong.FavoritedBy, (id) => {
                return db.Users.Find(id)!;
            });
            episodesDifference.Apply(dbSong.Episodes, (id) => {
                return db.Episodes.Find(id)!;
            });
            associacionsDifference.Apply(dbSong.Associacions, (id) => {
                return db.Associacions.Find(id)!;
            });

            return dbSong;
        }

        public override DiscordString ToDiscordString() {
            return DiscordString.Url(Name, $"https://www.youtube.com/watch?v={YoutubeId}") + " " + DiscordString.Code($"[{Id}]");
        }
    }
}
