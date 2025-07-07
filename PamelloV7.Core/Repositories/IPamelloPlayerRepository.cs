using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloPlayerRepository : IPamelloRepository<IPamelloPlayer>
{
    public IPamelloPlayer Create(IPamelloUser user, string name);
}
