using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands.Base;

public abstract class PamelloCommand
{
    public readonly IPamelloUser ScopeUser;
    public readonly IServiceProvider Services;
}
