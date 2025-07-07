using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloSongRepository : IPamelloRepository<IPamelloSong>
{
    public Task<IPamelloSong> AddAsync(string youtubeId, IPamelloUser adder);
    public IPamelloSong GetRandom();
    public IPamelloSong GetByYoutubeId(string youtubeId);

    public IEnumerable<IPamelloSong> Search(
        string querry,
        IPamelloUser scopeUser,
        IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null
    );
}
