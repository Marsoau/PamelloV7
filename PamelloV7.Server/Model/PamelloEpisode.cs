using PamelloV7.DAL.Entity;

namespace PamelloV7.Server.Model
{
    public class PamelloEpisode : PamelloEntity<DatabaseEpisode>
    {
        public override int Id {
            get => _entity.Id;
        }
        public override string Name {
            get => _entity.Name;
            set {
                _entity.Name = value;
                Save();

                //updated
            }
        }
        public int Start {
            get => _entity.Start;
            set {
                if (_entity.Start == value) return;

                _entity.Start = value;
                Save();

                //updated
            }
        }
        public bool Skip {
            get => _entity.Skip;
            set {
                if (_entity.Skip == value) return;

                _entity.Skip = value;
                Save();

                //updated
            }
        }

        public PamelloSong Song {
            get => _songs.GetRequired(_entity.Song.Id);
        }

        public PamelloEpisode(IServiceProvider services,
            DatabaseEpisode databaseEpisode
        ) : base(databaseEpisode, services) {

        }

        public override object GetDTO() => throw new NotImplementedException();
    }
}
