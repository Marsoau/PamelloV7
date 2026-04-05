using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player;

[DiscordCommand("player de-select", "Deselect a player")]
public partial class PlayerDeSelect
{
    public async Task Execute() {
        var previous = SelectedPlayer;
        
        if (previous is not null) Command<Framework.Commands.PlayerSelect>().Execute(null);
        
        await RespondAsync("Player De-Select", previous is not null ? "Player de-selected" : "No player selected");
    }
}
