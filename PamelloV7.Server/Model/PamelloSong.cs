using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Services;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;
using PamelloV7.DAL;
using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Server.Config;
using PamelloV7.Server.Model.Difference;
using PamelloV7.Server.Repositories.Database;

namespace PamelloV7.Server.Model
{
    public class PamelloSong : PamelloEntity<DatabaseSong>, IPamelloSong
    {
        private readonly YoutubeDownloadService _downloader;

        private string _name;
        private string _youtubeId;
        private string _coverUrl;
        private int _playCount;
        private DateTime _addedAt;
        private IPamelloUser _addedBy;

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
                var file = new FileInfo($@"{PamelloServerConfig.Root.DataPath}/Music/{Id}.opus");

                if (!file.Exists) return false;
                if (_downloader.IsDownloading(this)) return false;
                if (file.Length == 0) return false;

                return true;
            }
        }

        public IPamelloUser AddedBy {
            get => _addedBy;
        }

        private List<IPamelloUser> _favoritedBy;
        private List<IPamelloEpisode> _episodes;
        private HashSet<IPamelloPlaylist> _playlists;
        private List<string> _associations;

        public IReadOnlyList<IPamelloUser> FavoritedBy {
            get => _favoritedBy;
        }

        public IReadOnlyList<IPamelloEpisode> Episodes {
            get => _episodes;
        }

        public IReadOnlyList<IPamelloPlaylist> Playlists {
            get => _playlists.ToList();
        }

        public IReadOnlyList<string> Associations {
            get => _associations;
        }

        public IEnumerable<int> FavoriteByIds {
            get => _favoritedBy.Select(e => e.Id);
        }
        public IEnumerable<int> EpisodesIds {
            get {
                var list = new List<IPamelloEpisode>(_episodes);
                list.Sort((a, b) => {
                    if (a == null && b == null) return 0;
                    if (a == null) return -1;
                    if (b == null) return 1;

                    if (a.Start.TotalSeconds > b.Start.TotalSeconds) return 1;
                    if (a.Start.TotalSeconds < b.Start.TotalSeconds) return -1;
                    return 0;
                });
                return list.Select(databaseEpisode => databaseEpisode.Id);
            }
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

        protected override void InitBase() {
            if (DatabaseEntity is null) return;

            _addedBy = _users.GetRequired(DatabaseEntity.AddedBy.Id);

            _favoritedBy = DatabaseEntity.FavoriteBy.Select(e => _users.Get(e.Id)).OfType<IPamelloUser>().ToList();
            _episodes = DatabaseEntity.Episodes.Select(e => base._episodes.Get(e.Id)).OfType<IPamelloEpisode>().ToList();
            _playlists = DatabaseEntity.PlaylistEntries
                .Where(entry => entry.PlaylistId == Id)
                .Select(e => base._playlists.Get(e.Id)).OfType<IPamelloPlaylist>().ToHashSet();
            _associations = DatabaseEntity.Associations.Where(e => e.Song.Id == Id).Select(e => e.Association).ToList();
        }

        public void AddAssociation(string association) {
            DatabaseAssociation.EnsureNotReserved(association);

            var db = GetDatabase();

            var databaseAssociation = db.Associations.Find(association);
            if (databaseAssociation is not null) {
                if (databaseAssociation.Song.Id == Id)
                    throw new PamelloException($"Association \"{association}\" already exist this song");

                throw new PamelloException($"Association \"{association}\" already exist for another song");
            }

            databaseAssociation = new DatabaseAssociation() {
                Association = association,
                Song = db.Songs.Find(Id)
            };

            db.Associations.Add(databaseAssociation);
            db.SaveChanges();

            _associations.Add(association);

            _events.Broadcast(new SongAssociacionsUpdated() {
                SongId = Id,
                Associacions = Associations
            });
        }

        public void RemoveAssociation(string association) {
            var db = GetDatabase();

            if (!_associations.Contains(association)) {
                throw new PamelloException("This song doesnt contain that association");
            }

            var databaseAssociation = db.Associations.Find(association);

            if (databaseAssociation is not null) {
                db.Associations.Remove(databaseAssociation);
                db.SaveChanges();
            }

            _associations.Remove(association);

            _events.Broadcast(new SongAssociacionsUpdated() {
                SongId = Id,
                Associacions = Associations
            });
        }

        public IPamelloEpisode AddEpisode(AudioTime start, string name, bool autoSkip) {
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

            var episode = ((PamelloEpisodeRepository)base._episodes).Load(databaseEpisode); //CHANGEEEE
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
        public void RemoveEpisode(IPamelloEpisode episode) {
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

        public void MakeFavorite(IPamelloUser user) {
            if (_favoritedBy.Contains(user)) return;

            _favoritedBy.Add(user);
            Save();

            user.AddFavoriteSong(this);
            _events.Broadcast(new SongFavoriteByIdsUpdated() {
                SongId = Id,
                FavoriteByIds = FavoriteByIds
            });
        }
        public void UnmakeFavorite(IPamelloUser user) {
            if (!_favoritedBy.Contains(user)) return;

            _favoritedBy.Remove(user);
            Save();

            user.RemoveFavoriteSong(this);
            _events.Broadcast(new SongFavoriteByIdsUpdated() {
                SongId = Id,
                FavoriteByIds = FavoriteByIds
            });
        }

        public void AddToPlaylist(IPamelloPlaylist playlist, int? position = null, bool fromInside = false) {
            if (!_playlists.Add(playlist)) return;
            Save();

            if (!fromInside) playlist.AddSong(this, position, true);
            _events.Broadcast(new SongPlaylistsIdsUpdated() {
                SongId = Id,
                PlaylistsIds = PlaylistsIds,
            });
        }
        public void RemoveFromPlaylist(IPamelloPlaylist playlist, bool fromInside = false) {
            if (_playlists.Contains(playlist)) return;

            if (!_playlists.Remove(playlist)) return;
            Save();

            if (!fromInside) playlist.RemoveSong(this, true);
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

                Associations = Associations,
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
                .Include(databaseSong => databaseSong.PlaylistEntries)
                .Include(databaseSong => databaseSong.FavoriteBy)
                .Include(databaseSong => databaseSong.Episodes)
                .Include(databaseSong => databaseSong.Associations)
                .AsSplitQuery()
                .FirstOrDefault();
            if (dbSong is null) throw new PamelloDatabaseSaveException();

            //var addedBy = db.Users.Find(_addedBy.Id);
            //if (addedBy is null) throw new PamelloDatabaseSaveException();

            dbSong.Name = Name;
            dbSong.YoutubeId = YoutubeId;
            dbSong.CoverUrl = CoverUrl;
            dbSong.PlayCount = PlayCount;

            var dbEpisodesIds = dbSong.Episodes.Select(episode => episode.Id);
            var dbAssociationsValues = dbSong.Associations.Select(association => association.Association);

            var episodesDifference = DifferenceResult<int>.From(
                dbEpisodesIds, 
                EpisodesIds,
                withMoved: true
            );
            var associationsDifference = DifferenceResult<string>.From(
                dbAssociationsValues, 
                Associations,
                withMoved: true
            );

            episodesDifference.ExcludeMoved();
            associationsDifference.ExcludeMoved();

            episodesDifference.Apply(dbSong.Episodes, id
                => db.Episodes.Find(id)!);
            associationsDifference.Apply(dbSong.Associations, id
                => db.Associations.Find(id)!);

            return dbSong;
        }

        public override DiscordString ToDiscordString() {
            return DiscordString.Url(Name, $"https://www.youtube.com/watch?v={YoutubeId}") + " " + DiscordString.Code($"[{Id}]");
        }
    }
}
