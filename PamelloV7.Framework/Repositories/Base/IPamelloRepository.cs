using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.History.Records;

namespace PamelloV7.Framework.Repositories.Base;

public interface IPamelloRepository<TEntity>
    where TEntity : IPamelloEntity
{
    public TEntity? Get(int id);
    public TEntity GetRequired(int id);

    public IHistoryRecord Delete(TEntity entity, IPamelloUser? scopeUser);
}