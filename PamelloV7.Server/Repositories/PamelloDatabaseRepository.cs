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

        public event Action? BeforeInit;
        public event Action<int, int>? OnInitProgress;
        public event Action? OnInit;

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
            return _loaded[Random.Shared.Next(0, _loaded.Count)];
        }
        public TPamelloEntity GetRequired(int id)
			=> Get(id) ?? throw new PamelloException($"Cant find required {typeof(TPamelloEntity).Name} with id {id}");
        public virtual TPamelloEntity? Get(int id) {
            var pamelloEntity = _loaded.FirstOrDefault(e => e.Id == id);
            if (pamelloEntity is not null) return pamelloEntity;

            var entities = GetEntities();

            var databaseEntity = entities.Find(e => e.Id == id);
            if (databaseEntity is null) return null;

            return Load(databaseEntity);
        }

        public async Task<TPamelloEntity> GetByValueRequired(string value, PamelloUser? scopeUser = null)
            => await GetByValue(value, scopeUser) ?? throw new PamelloException($"Cant find required {typeof(TPamelloEntity).Name} with value \"{value}\"");
        public abstract Task<TPamelloEntity?> GetByValue(string value, PamelloUser? scopeUser = null);

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
                LoadBase(databaseEntity);
                OnLoadingProgress?.Invoke(++count, total);
            }

            OnLoaded?.Invoke();
        }
        public Task LoadAllAsync() {
            return Task.Run(LoadAll);
        }

        public void InitAll() {
            BeforeInit?.Invoke();

            var count = 0;
            var total = _loaded.Count;

            foreach (var entity in _loaded) {
                entity.Init();
                OnInitProgress?.Invoke(++count, total);
            }

            OnInit?.Invoke();
        }
        public Task InitAllAsync() {
            return Task.Run(InitAll);
        }

        public TPamelloEntity? Load(TDatabaseEntity databaseEntity, bool doInit = true) {
            var entity = LoadBase(databaseEntity);

            if (doInit) entity?.Init();

            return entity;
        }
        protected abstract TPamelloEntity? LoadBase(TDatabaseEntity databaseEntity);

        public abstract void Delete(TPamelloEntity entity);
    }
}
