using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloPlayerRepository : IPamelloRepository<IPamelloPlayerOld>
{
    public IPamelloPlayerOld Create(IPamelloUser user, string name);
}
