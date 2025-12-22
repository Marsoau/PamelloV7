using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Repositories.Base;

public interface IPamelloRepository<TEntity>
    where TEntity : IPamelloEntity
{
    public TEntity? Get(int id);
    public IEnumerable<TEntity> GetLoaded();
    public TEntity GetRequired(int id);

    public Task<TEntity?> GetByValue(string value, IPamelloUser? scopeUser);
    public Task<TEntity> GetByValueRequired(string value, IPamelloUser? scopeUser);

    public Task<IEnumerable<TEntity>> SearchAsync(string query, IPamelloUser? scopeUser = null);

    public void Delete(TEntity entity);
}