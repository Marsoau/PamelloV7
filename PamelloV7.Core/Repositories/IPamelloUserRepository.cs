using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloUserRepository : IPamelloDatabaseRepository<IPamelloUser>, IPamelloService
{
    public IPamelloUser? GetByToken(Guid token);
    public IPamelloUser? GetByDiscord(ulong discordId, bool createIfNotFound = true);
}
