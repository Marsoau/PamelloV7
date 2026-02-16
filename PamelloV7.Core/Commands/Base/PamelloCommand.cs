using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Other;

namespace PamelloV7.Core.Commands.Base;

public abstract class PamelloCommand
{
    public readonly IPamelloUser ScopeUser;
    public readonly IServiceProvider Services;
    
    protected IPamelloPlayer RequiredSelectedPlayer => ScopeUser.RequiredSelectedPlayer;
    protected IPamelloQueue RequiredQueue => RequiredSelectedPlayer.RequiredQueue;
}
