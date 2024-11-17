using PamelloV7.DAL.Entity;

namespace PamelloV7.Server.Model
{
    public class PamelloSong : PamelloEntity<DatabaseSong>
    {
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

                if (file.Length == 0) return false;
                return true;
            }
        }

        public PamelloUser AddedBy {
            get => _users.GetRequired(Entity.AddedBy.Id);
        }

        public IReadOnlyList<PamelloUser> FavoritedBy {
            get => throw new NotImplementedException();
        }

        public IReadOnlyList<PamelloEpisode> Episodes {
            get => throw new NotImplementedException();
        }

        public IReadOnlyList<PamelloPlaylist> Playlists {
            get => throw new NotImplementedException();
        }

        public IReadOnlyList<string> Associacions {
            get => Entity.Associacions.Select(associacion => associacion.Associacion).ToList();
        }

        public PamelloSong(IServiceProvider services,
            DatabaseSong databaseSong
        ) : base(databaseSong, services) {

        }

        public override object GetDTO() => throw new NotImplementedException();
    }
}
