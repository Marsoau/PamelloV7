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
            client.Users
        ];
    }
    
    public IRemoteEntity? GetSingle(Type type, int id) {
        return null;
    }

    public SafeStoredEntities<TRemoteEntity> Get<TRemoteEntity>(IEnumerable<int> ids) where TRemoteEntity : class, IRemoteEntity {
        throw new NotImplementedException();
    }

    public Task<SafeStoredEntities<TRemoteEntity>> GetAsync<TRemoteEntity>(string query) where TRemoteEntity : class, IRemoteEntity {
        throw new NotImplementedException();
    }
}
