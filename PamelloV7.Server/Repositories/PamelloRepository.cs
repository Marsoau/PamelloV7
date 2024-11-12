using PamelloV7.DAL;
using PamelloV7.Server.Model;
using PamelloV7.Server.Exceptions;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories
{
    public abstract class PamelloRepository<TPamelloEntity, TDatabaseEntity>
        where TPamelloEntity : PamelloEntity<TDatabaseEntity>
        where TDatabaseEntity : DatabaseEntity
    {
        protected readonly IServiceProvider _services;

        protected readonly PamelloEventsService _events;

        protected readonly DatabaseContext _database;

        protected readonly List<TPamelloEntity> _loaded;
        protected readonly List<TDatabaseEntity> _nonloaded;

        public PamelloRepository(IServiceProvider services) {
            _services = services;

            _events = services.GetRequiredService<PamelloEventsService>();

            _database = services.GetRequiredService<DatabaseContext>();

            _loaded = new List<TPamelloEntity>();
            _nonloaded = LoadDatabaseEntities();
        }

        public abstract List<TDatabaseEntity> LoadDatabaseEntities();

        public TPamelloEntity GetRequired(int id)
			=> Get(id) ?? throw new PamelloException($"Cant find required {typeof(TPamelloEntity).Name} with id {id}");
        public virtual TPamelloEntity? Get(int id) {
            var pamelloEntity = _loaded.FirstOrDefault(e => e.Id == id);
            if (pamelloEntity is not null) return pamelloEntity;

            var databaseEntity = _nonloaded.FirstOrDefault(e => e.Id == id);
            if (databaseEntity is null) return null;

            return Load(databaseEntity);
        }

        public abstract TPamelloEntity? Load(TDatabaseEntity databaseEntity);

        public abstract void Delete(int id);
    }
}
