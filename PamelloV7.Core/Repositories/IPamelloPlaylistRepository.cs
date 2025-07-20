using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloPlaylistRepository : IPamelloDatabaseRepository<IPamelloPlaylist>, IPamelloService
{
    public IEnumerable<IPamelloPlaylist> Search(
        string querry,
        IPamelloUser scopeUser,
        IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null
    );
}
