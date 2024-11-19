using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public interface IPamelloRepository<TEntity> where TEntity : IDTOEntity
    {
        public TEntity GetRequired(int id);
        public TEntity Get(int id);
    }
}
