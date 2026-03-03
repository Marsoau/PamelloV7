using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Entities.Base;
using PamelloV7.Wrapper.Query.Base;
using PamelloV7.Wrapper.Repositories;
using PamelloV7.Wrapper.Repositories.Base;

namespace PamelloV7.Wrapper.Query;

public class RemoteEntityQueryService : IRemoteEntityQueryService
{
    private readonly IRemoteRepository[] _repositories;
    
    public RemoteEntityQueryService(PamelloClient client) {
        _repositories = [
            client.Users,
            client.Songs,
            client.Episodes,
            client.Playlists,
            client.Players
        ];
    }

    private IRemoteRepository GetRepositoryRequired(Type type)
        => GetRepository(type) ?? throw new Exception($"No repository for type {type.Name}");
    private IRemoteRepository? GetRepository(Type type) {
        return _repositories.FirstOrDefault(x => x.EntityType == type);
    }

    public IRemoteEntity? GetSingle(Type type, int id)
        => GetRepositoryRequired(type).GetSingle(id);

    public Task<IRemoteEntity?> GetSingleAsync(Type type, int id)
        => GetRepositoryRequired(type).GetSingleAsync(id);

    public Task<IRemoteEntity?> GetSingleAsync(Type type, string query)
        => GetRepositoryRequired(type).GetSingleAsync(query);

    public Task<IEnumerable<IRemoteEntity>> GetAsync(Type type, string query)
        => GetRepositoryRequired(type).GetAsync(query);

    public Task<IEnumerable<int>> GetIdsAsync(Type type, string query)
        => GetRepositoryRequired(type).GetIdsAsync(query);

    public Task<IEnumerable<IRemoteEntity>> GetAsync(string query) {
        throw new NotImplementedException();
    }
}
