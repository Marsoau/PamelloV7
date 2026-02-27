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

    private IRemoteRepository GetRepositoryRequired(Type type)
        => GetRepository(type) ?? throw new Exception($"no repository for type {type.Name}");
    private IRemoteRepository? GetRepository(Type type) {
        return _repositories.FirstOrDefault(x => x.EntityType == type);
    }
    
    public IRemoteEntity? GetSingle(Type type, int id) {
        var repository = GetRepositoryRequired(type);
        
        return repository.Get(id);
    }

    public IEnumerable<TRemoteEntity> Get<TRemoteEntity>(IEnumerable<int> ids) where TRemoteEntity : class, IRemoteEntity {
        var repository = GetRepositoryRequired(typeof(TRemoteEntity));
        
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TRemoteEntity>> GetAsync<TRemoteEntity>(string query) where TRemoteEntity : class, IRemoteEntity {
        var repository = GetRepositoryRequired(typeof(TRemoteEntity));
        
        throw new NotImplementedException();
    }
}
