using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloSongRepository : IPamelloDatabaseRepository<IPamelloSong>, IPamelloService
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
