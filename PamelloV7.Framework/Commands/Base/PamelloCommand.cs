using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;

namespace PamelloV7.Framework.Commands.Base;

public abstract class PamelloCommand
{
    public readonly IPamelloUser ScopeUser = null!;
    public readonly IServiceProvider Services = null!;
    
    protected IPamelloPlayer RequiredSelectedPlayer => ScopeUser.RequiredSelectedPlayer;
    protected IPamelloQueue RequiredQueue => RequiredSelectedPlayer.RequiredQueue;
}
