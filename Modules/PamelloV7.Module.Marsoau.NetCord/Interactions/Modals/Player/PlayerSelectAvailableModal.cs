using Microsoft.Extensions.DependencyInjection;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Player;

[DiscordModal("Select Available Player")]

[AddSelect<IPamelloPlayer>("Player*", "Player")]

public partial class PlayerSelectAvailableModal
{
    public partial class Builder
    {
        public void Build() {
            var players = Services.GetRequiredService<IPamelloPlayerRepository>();
            Player.Options = players.GetAdded(ScopeUser, ScopeUser).Select(
                player => new StringMenuSelectOptionProperties(player.Name, player.Id.ToString())
            );
        }
    }
    
    public void Submit() {
        Command<PlayerSelect>().Execute(Player);
    }
}
