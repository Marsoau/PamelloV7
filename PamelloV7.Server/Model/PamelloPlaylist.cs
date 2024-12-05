using PamelloV7.Core.Exceptions;
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

        public bool IsProtected {
            get => Entity.IsProtected;
            set {
                if (Entity.IsProtected == value) return;

                Entity.IsProtected = value;
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

        public void AddSong(PamelloSong song) {
            if (Entity.Songs.Any(databaseSong => databaseSong.Id == song.Id)) {
                throw new PamelloException("Song is already present in that playlist");
            }

            Entity.Songs.Add(song.Entity);
            Save();
        }

        public int AddList(IReadOnlyList<PamelloSong> list) {
            int count = 0;

            foreach (var song in list) {
                if (Entity.Songs.Any(databaseSong => databaseSong.Id == song.Id)) continue;

                Entity.Songs.Add(song.Entity);

                count++;
            }

            if (count > 0) {
                Save();
            }

            return count;
        }

        public void RemoveSong(PamelloSong song) {
            if (!Entity.Songs.Any(databaseSong => databaseSong.Id == song.Id)) {
                throw new PamelloException("There is no that song in that playlist");
            }

            Entity.Songs.Remove(song.Entity);
            Save();
        }

        public override object? DTO => new { };
    }
}
