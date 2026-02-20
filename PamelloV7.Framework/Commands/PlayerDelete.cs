using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Framework.Commands;

public class PlayerDelete : PamelloCommand
{
    public void Execute() {
        var players = Services.GetRequiredService<IPamelloPlayerRepository>();
        players.Delete(ScopeUser.RequiredSelectedPlayer, ScopeUser);
    }
}
