using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Repositories;

public interface IRemoteRepository
{
    public string ProviderName { get; }
    public string RemoteInterfaceName { get; }
    public Type DtoType { get; }
    public Type EntityType { get; }
    
    public IRemoteEntity GetRequired(int id)
        => Get(id) ?? throw new PamelloException($"{EntityType.Name} with id {id} not found");
    public IRemoteEntity? Get(int id);
}
