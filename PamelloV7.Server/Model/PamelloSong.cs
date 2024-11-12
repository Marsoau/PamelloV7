using PamelloV7.DAL.Entity;

namespace PamelloV7.Server.Model
{
    public class PamelloSong : PamelloEntity<DatabaseSong>
    {
        public override int Id {
            get => _entity.Id;
        }
        public override string Name {
            get => _entity.Name;
            set {
                if (_entity.Name == value) return;

                _entity.Name = value;

                //updated
            }
        }
        public string YoutubeId {
            get => _entity.YoutubeId;
            set {
                _entity.YoutubeId = value;

                //updated
            }
        }
        public int PlayCount {
            get => _entity.PlayCount;
            set {
                if (_entity.PlayCount == value) return;

                _entity.PlayCount = value;

                //updated
            }
        }

        public PamelloUser AddedBy {
            get => _users.GetRequired(_entity.AddedBy.Id);
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
            get => _entity.Associacions.Select(associacion => associacion.Associacion).ToList();
        }

        public PamelloSong(IServiceProvider services,
            DatabaseSong databaseSong
        ) : base(databaseSong, services) {

        }

        public override object GetDTO() => throw new NotImplementedException();
    }
}
