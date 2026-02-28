using PamelloV7.Wrapper.Entities;
using PamelloV7.Wrapper.Repositories.Base;
using PamelloV7.Wrapper.Requests;

namespace PamelloV7.Wrapper.Repositories;

public class RemoteSongRepository : RemoteRepository<RemoteSong>
{
    public RemoteSongRepository(PamelloRequestsService requests) : base(requests) { }
}
