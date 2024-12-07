using PamelloV7.DAL;
using PamelloV7.Server.Model;
using PamelloV7.Core.Exceptions;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Services;
using PamelloV7.Core.Audio;

namespace PamelloV7.Server.Repositories
{
    public abstract class PamelloDatabaseRepository<TPamelloEntity, TDatabaseEntity> : IPamelloRepository<TPamelloEntity>
        where TPamelloEntity : PamelloEntity<TDatabaseEntity>
        where TDatabaseEntity : DatabaseEntity
    {
        protected readonly IServiceProvider _services;

        protected readonly PamelloEventsService _events;

        protected readonly DatabaseContext _database;

        protected readonly List<TPamelloEntity> _loaded;
        protected List<TDatabaseEntity> _nonloaded
            => LoadDatabaseEntities();

        public PamelloDatabaseRepository(IServiceProvider services) {
            _services = services;

            _events = services.GetRequiredService<PamelloEventsService>();

            _database = services.GetRequiredService<DatabaseContext>();

            _loaded = new List<TPamelloEntity>();
        }

        public abstract void InitServices();

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

        public async Task<TPamelloEntity> GetByValueRequired(string value, PamelloUser? scopeUser = null)
            => await GetByValue(value, scopeUser) ?? throw new PamelloException($"Cant find required {typeof(TPamelloEntity).Name} with value \"{value}\"");
        public abstract Task<TPamelloEntity?> GetByValue(string value, PamelloUser? scopeUser);

        protected Task<IEnumerable<TPamelloEntity>> Search(IEnumerable<TPamelloEntity> list, string querry, PamelloUser? scopeUser) {
            var results = new List<TPamelloEntity>();
            querry = querry.ToLower();

            foreach (var pamelloEntity in list) {
                if (pamelloEntity is null) continue;

                if (pamelloEntity.Name.ToLower().Contains(querry)) {
                    results.Add(pamelloEntity);
                }
            }

            return Task.FromResult<IEnumerable<TPamelloEntity>>(results);
        }
        public async Task<IEnumerable<TPamelloEntity>> Search(string querry, PamelloUser? scopeUser = null) {
            LoadAll();

            return await Search(_loaded, querry, scopeUser);
        }

        protected void LoadAll() {
            foreach (var databaseEntity in _nonloaded) {
                Load(databaseEntity);
            }
        }

        public abstract TPamelloEntity? Load(TDatabaseEntity databaseEntity);

        public abstract void Delete(int id);
    }
}
