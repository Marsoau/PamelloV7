using PamelloV7.DAL.Entity;

namespace PamelloV7.Server.Model
{
    public class PamelloPlaylist : PamelloEntity<DatabasePlaylist>
    {
        public override int Id {
            get => Entity.Id;
        }

        public override string Name {
            get => Entity.Name;
            set {
                if (Entity.Name == value) return;

                Entity.Name = value;
            }
        }

        public PamelloUser OwnedBy {
            get => _users.GetRequired(Entity.Owner.Id);
        }

        public IReadOnlyList<PamelloSong> Songs {
            get => Entity.Songs.Select(song => _songs.GetRequired(song.Id)).ToList();
        }
        public IReadOnlyList<PamelloUser> FavoritedBy {
            get => Entity.FavoritedBy.Select(user => _users.GetRequired(user.Id)).ToList();
        }

        public PamelloPlaylist(IServiceProvider services,
            DatabasePlaylist databasePlaylist
        ) : base(databasePlaylist, services) {

        }

        public override object? DTO => new { };
    }
}
