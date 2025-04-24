using PamelloV7.DAL;
using PamelloV7.Server.Model;
using PamelloV7.Core.Exceptions;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Services;
using PamelloV7.Core.Audio;
using Microsoft.EntityFrameworkCore;

namespace PamelloV7.Server.Repositories
{
    public abstract class PamelloDatabaseRepository<TPamelloEntity, TDatabaseEntity> : IPamelloRepository<TPamelloEntity>
        where TPamelloEntity : PamelloEntity<TDatabaseEntity>
        where TDatabaseEntity : DatabaseEntity
    {
        protected readonly IServiceProvider _services;

        public event Action? BeforeLoading;
        public event Action<int, int>? OnLoadingProgress;
        public event Action? OnLoaded;

        protected PamelloEventsService _events { get; private set; }

        protected readonly List<TPamelloEntity> _loaded;
        protected DatabaseContext GetDatabase() {
            var db = _services.GetRequiredService<DatabaseContext>(); 
            //Console.WriteLine($"created new datacase {db.GetHashCode()}");

            return db;
        }

        public PamelloDatabaseRepository(IServiceProvider services) {
            _services = services;

            _loaded = new List<TPamelloEntity>();
        }

        public virtual void InitServices() {
            _events = _services.GetRequiredService<PamelloEventsService>();
        }

        public abstract List<TDatabaseEntity> ProvideEntities();
        protected List<TDatabaseEntity> GetEntities() {
            return ProvideEntities();
        }

        public TPamelloEntity GetRandom() {
            var entities = GetEntities();

            return Get(Random.Shared.Next(0, entities.Count));
        }
        public TPamelloEntity GetRequired(int id)
			=> Get(id) ?? throw new PamelloException($"Cant find required {typeof(TPamelloEntity).Name} with id {id}");
        public virtual TPamelloEntity? Get(int id) {
            Console.WriteLine($"Loaded for {typeof(TPamelloEntity)}: {_loaded.Count}");
            var db = GetEntities();

            var pamelloEntity = _loaded.FirstOrDefault(e => e.Id == id);
            if (pamelloEntity is not null) return pamelloEntity;

            Console.WriteLine("No entity");

            var databaseEntity = db.Find(e => e.Id == id);
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
            return await Search(_loaded, querry, scopeUser);
        }

        public void LoadAll() {
            BeforeLoading?.Invoke();

            var entities = GetEntities();

            var count = 0;
            var total = entities.Count;

            foreach (var databaseEntity in entities) {
                Load(databaseEntity);
                OnLoadingProgress?.Invoke(++count, total);
            }

            OnLoaded?.Invoke();
        }

        public abstract TPamelloEntity? Load(TDatabaseEntity databaseEntity);

        public abstract void Delete(int id);
    }
}
