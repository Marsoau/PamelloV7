using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;

namespace PamelloV7.Core.Commands;

public class PlayerDelete : PamelloCommand
{
    public void Execute() {
        var players = Services.GetRequiredService<IPamelloPlayerRepository>();
        players.Delete(ScopeUser.RequiredSelectedPlayer);
    }
}
