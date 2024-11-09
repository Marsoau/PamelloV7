using PamelloV7.DAL;

namespace PamelloV7.Server.Model
{
    public abstract class PamelloEntity<T> where T : class
    {
        protected readonly T Entity;

        private readonly DatabaseContext _database;

        public abstract int Id { get; }
        public abstract string Name { get; set; }

        public PamelloEntity(T databaseEntity, IServiceProvider services) {
            _database = services.GetRequiredService<DatabaseContext>();

            Entity = databaseEntity;
        }

        protected void Save() => _database.SaveChanges();
        public abstract object GetDTO();
    }
}
