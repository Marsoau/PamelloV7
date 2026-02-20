using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Services;

public interface IPamelloCommandsService : IPamelloService
{
    public TCommand Get<TCommand>(IPamelloUser scopeUser) where TCommand : PamelloCommand, new();
    public PamelloCommand Get(Type commandType, IPamelloUser scopeUser);
    public Task<object?> ExecutePathAsync(string commandPath, IPamelloUser scopeUser);
}
