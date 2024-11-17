using PamelloV7.DAL;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model
{
    public abstract class PamelloEntity<T> where T : DatabaseEntity
    {
        protected internal readonly T Entity;

        protected readonly PamelloEventsService _events;

        private readonly DatabaseContext _database;

        protected readonly PamelloSongRepository _songs;
        protected readonly PamelloEpisodeRepository _episodes;
        protected readonly PamelloPlaylistRepository _playlists;
        protected readonly PamelloUserRepository _users;

        public abstract int Id { get; }
        public abstract string Name { get; set; }

        public PamelloEntity(T databaseEntity, IServiceProvider services) {
            _events = services.GetRequiredService<PamelloEventsService>();
            _database = services.GetRequiredService<DatabaseContext>();

            _songs = services.GetRequiredService<PamelloSongRepository>();
            _episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<PamelloPlaylistRepository>();
            _users = services.GetRequiredService<PamelloUserRepository>();

            Entity = databaseEntity;
        }

        protected void Save() => _database.SaveChanges();
        public abstract object GetDTO();

        public override string ToString() {
            return $"[{Id}] {Name}";
        }
    }
}
