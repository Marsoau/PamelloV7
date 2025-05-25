using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public interface IPamelloRepository<TEntity> : IDisposable where TEntity : IPamelloEntity
    {
        public void InitServices();
        
        public TEntity? Get(int id);
        public Task<TEntity?> GetByValue(string value, PamelloUser? scopeUser);
        public TEntity GetRequired(int id);
        public Task<TEntity> GetByValueRequired(string value, PamelloUser? scopeUser);

        public Task<IEnumerable<TEntity>> SearchAsync(string querry, PamelloUser? scopeUser);
    }
}
