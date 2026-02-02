using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;

namespace PamelloV7.Core.Commands;

public class PlayerCreate : PamelloCommand
{
    public IPamelloPlayer Execute(string name) {
        var players = Services.GetRequiredService<IPamelloPlayerRepository>();
        var player = players.Create(name, ScopeUser);
        
        ScopeUser.SelectedPlayer = player;
        
        return player;
    }
}
