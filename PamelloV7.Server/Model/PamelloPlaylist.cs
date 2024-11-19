using PamelloV7.DAL.Entity;

namespace PamelloV7.Server.Model
{
    public class PamelloPlaylist : PamelloEntity<DatabasePlaylist>
    {
        public override int Id => throw new NotImplementedException();
        public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IReadOnlyList<PamelloSong> Songs {
            get => Entity.Songs.Select(song => _songs.GetRequired(song.Id)).ToList();
        }

        public PamelloPlaylist(IServiceProvider services,
            DatabasePlaylist databasePlaylist
        ) : base(databasePlaylist, services) {

        }

        public override object? DTO => new { };
    }
}
