using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloUserRepository : IPamelloRepository<IPamelloUser>
{
    public IPamelloUser? GetByToken(Guid token);
    public IPamelloUser? GetByDiscord(ulong discordId, bool createIfNotFound = true);
}
