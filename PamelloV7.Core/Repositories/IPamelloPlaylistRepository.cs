using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloPlaylistRepository : IPamelloRepository<IPamelloPlaylist>
{
    public IEnumerable<IPamelloPlaylist> Search(
        string querry,
        IPamelloUser scopeUser,
        IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null
    );
}
