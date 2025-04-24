using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Services;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;

namespace PamelloV7.Server.Model
{
    public class PamelloSong : PamelloEntity<DatabaseSong>
    {
        private readonly YoutubeDownloadService _downloader;

        public override int Id {
            get => Entity.Id;
        }
        public override string Name {
            get => Entity.Name;
            set {
                if (Entity.Name == value) return;

                Entity.Name = value;

                _events.Broadcast(new SongNameUpdated() {
                    SongId = Id,
                    Name = Name,
                });
            }
        }
        public string YoutubeId {
            get => Entity.YoutubeId;
        }
        public string CoverUrl {
            get => Entity.CoverUrl;
            set {
                Entity.CoverUrl = value;

                _events.Broadcast(new SongCoverUrlUpdated() {
                    SongId = Id,
                    CoverUrl = CoverUrl
                });
            }
        }
        public int PlayCount {
            get => Entity.PlayCount;
            set {
                if (Entity.PlayCount == value) return;

                Entity.PlayCount = value;

                _events.Broadcast(new SongPlayCountUpdated() {
                    SongId = Id,
                    PlayCount = PlayCount
                });
            }
        }
        public DateTime AddedAt {
            get => Entity.AddedAt;
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
            get => _users.GetRequired(Entity.AddedBy.Id);
        }

        public IReadOnlyList<PamelloUser> FavoritedBy {
            get => Entity.FavoritedBy.Select(user => _users.GetRequired(user.Id)).ToList();
        }

        public IReadOnlyList<PamelloEpisode> Episodes {
            get => EpisodesIds.Select(_episodes.GetRequired).ToList();
        }

        public IReadOnlyList<PamelloPlaylist> Playlists {
            get => Entity.Playlists.Select(playlist => _playlists.GetRequired(playlist.Id)).ToList();
        }

        public IReadOnlyList<string> Associacions {
            get => Entity.Associacions.Select(associacion => associacion.Associacion).ToList();
        }

        public IEnumerable<int> FavoriteByIds {
            get => Entity.FavoritedBy.Select(databaseUser => databaseUser.Id);
        }
        public IEnumerable<int> EpisodesIds {
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
        }
        public IEnumerable<int> PlaylistsIds {
            get => Entity.Playlists.Select(databasePlaylist => databasePlaylist.Id);
        }

        public PamelloSong(IServiceProvider services,
            DatabaseSong databaseSong
        ) : base(databaseSong, services) {
            _downloader = services.GetRequiredService<YoutubeDownloadService>();
        }

        public void AddAssociacion(string associacion) {
            if (DatabaseAssociacion.Reserved.Contains(associacion)) {
                throw new PamelloException($"Associacion \"{associacion}\" is reserved");
            }

            var db = GetDatabase();

            var databaseAssociacion = db.Associacions.Find(associacion);
            if (databaseAssociacion is not null) {
                if (databaseAssociacion.Song.Id == Entity.Id)
                    throw new PamelloException($"Associacion \"{associacion}\" already exist this song");

                throw new PamelloException($"Associacion \"{associacion}\" already exist for another song");
            }

            databaseAssociacion = new DatabaseAssociacion() {
                Associacion = associacion,
                Song = Entity
            };

            db.Associacions.Add(databaseAssociacion);
            db.SaveChanges();

            _events.Broadcast(new SongAssociacionsUpdated() {
                SongId = Id,
                Associacions = Associacions
            });
        }

        public void RemoveAssociacion(string associacion, bool removeGlobaly = false) {
            var db = GetDatabase();

            var databaseAssociacion = db.Associacions.Find(associacion);

            if (databaseAssociacion is null || (!removeGlobaly && databaseAssociacion.Song.Id != Entity.Id)) {
                throw new PamelloException("This song doesnt contain");
            }

            db.Associacions.Remove(databaseAssociacion);
            db.SaveChanges();

            _events.Broadcast(new SongAssociacionsUpdated() {
                SongId = Id,
                Associacions = Associacions
            });
        }

        public PamelloEpisode AddEpisode(AudioTime start, string name) {
            var episode = _episodes.Create(this, start, name, false);
            return episode;
        }
        public void RemoveEpisode(int position) {
            var episode = Episodes.ElementAtOrDefault(position);
            if (episode is null) throw new PamelloException($"Episode in position {position} was not found");

            _episodes.Delete(episode.Id);
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloSongDTO {
                Id = Id,
                Name = Name,
                YoutubeId = YoutubeId,
                CoverUrl = CoverUrl,
                PlayCount = PlayCount,
                AddedById = Entity.AddedBy.Id,
                AddedAt = AddedAt,

                Associacions = Associacions,
                FavoriteByIds = FavoriteByIds,
                PlaylistsIds = PlaylistsIds,
                EpisodesIds = EpisodesIds,

                IsDownloading = _downloader.IsDownloading(this),
                DownloadProgress = _downloader.GetProgress(this),
            };
        }

        public override DiscordString ToDiscordString() {
            return DiscordString.Url(Name, $"https://www.youtube.com/watch?v={YoutubeId}") + " " + DiscordString.Code($"[{Id}]");
        }
    }
}
