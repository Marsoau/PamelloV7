using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IPamelloCommandsService : IPamelloService
{
    public TCommand Get<TCommand>(IPamelloUser scopeUser) where TCommand : PamelloCommand, new();
    public PamelloCommand Get(Type commandType, IPamelloUser scopeUser);
    public Task<object?> ExecutePathAsync(string commandPath, IPamelloUser scopeUser);
}
