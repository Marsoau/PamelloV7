using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Repositories.Base;

public interface IPamelloRepository<TEntity>
    where TEntity : IPamelloEntity
{
    public TEntity? Get(int id);
    public TEntity GetRequired(int id);

    public void Delete(TEntity entity);
}