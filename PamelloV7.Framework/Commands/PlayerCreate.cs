using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Framework.Commands;

public class PlayerCreate : PamelloCommand
{
    public IPamelloPlayer Execute(string name) {
        var players = Services.GetRequiredService<IPamelloPlayerRepository>();
        var player = players.Create(name, ScopeUser);
        
        ScopeUser.SelectPlayer(player);
        
        return player;
    }
}
