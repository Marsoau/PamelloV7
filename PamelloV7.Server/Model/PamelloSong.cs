using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Services;

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

                //updated
            }
        }
        public string YoutubeId {
            get => Entity.YoutubeId;
            set {
                Entity.YoutubeId = value;

                //updated
            }
        }
        public int PlayCount {
            get => Entity.PlayCount;
            set {
                if (Entity.PlayCount == value) return;

                Entity.PlayCount = value;

                //updated
            }
        }

        public bool IsDownloaded {
            get {
                var file = new FileInfo($@"{AppContext.BaseDirectory}Data\Music\{Id}.opus");

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
            get => Entity.Episodes.Select(episode => _episodes.GetRequired(episode.Id)).ToList();
        }

        public IReadOnlyList<PamelloPlaylist> Playlists {
            get => Entity.Playlists.Select(playlist => _playlists.GetRequired(playlist.Id)).ToList();
        }

        public IReadOnlyList<string> Associacions {
            get => Entity.Associacions.Select(associacion => associacion.Associacion).ToList();
        }

        public PamelloSong(IServiceProvider services,
            DatabaseSong databaseSong
        ) : base(databaseSong, services) {
            _downloader = services.GetRequiredService<YoutubeDownloadService>();
        }

        public override object DTO => new {
            Name = "gay"
        };

        public override DiscordString ToDiscordString() {
            return DiscordString.Url(Name, $"https://www.youtube.com/watch?v={YoutubeId}") + " " + DiscordString.Code($"[{Id}]");
        }
    }
}
