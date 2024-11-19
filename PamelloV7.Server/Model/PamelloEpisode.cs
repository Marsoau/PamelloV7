using PamelloV7.DAL.Entity;

namespace PamelloV7.Server.Model
{
    public class PamelloEpisode : PamelloEntity<DatabaseEpisode>
    {
        public override int Id {
            get => Entity.Id;
        }
        public override string Name {
            get => Entity.Name;
            set {
                Entity.Name = value;
                Save();

                //updated
            }
        }
        public int Start {
            get => Entity.Start;
            set {
                if (Entity.Start == value) return;

                Entity.Start = value;
                Save();

                //updated
            }
        }
        public bool Skip {
            get => Entity.Skip;
            set {
                if (Entity.Skip == value) return;

                Entity.Skip = value;
                Save();

                //updated
            }
        }

        public PamelloSong Song {
            get => _songs.GetRequired(Entity.Song.Id);
        }

        public PamelloEpisode(IServiceProvider services,
            DatabaseEpisode databaseEpisode
        ) : base(databaseEpisode, services) {

        }

        public override object? DTO => new { };

        public override string ToString() {
            return $"{base.ToString()} ({(Skip ? "Skip" : "Play")})";
        }
    }
}
