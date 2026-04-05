using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player;

[DiscordCommand("player select", "Select a player")]
public partial class PlayerSelect
{
    public async Task Execute(
        [PlayerDescription] IPamelloPlayer player
    ) {
        if (player == SelectedPlayer) {
            await RespondAsync("Player Already Selected", SelectedPlayer.ToDiscordString, () => [player]);
            return;
        }
        
        var selectedPlayer = Command<Framework.Commands.PlayerSelect>().Execute(player);
        if (selectedPlayer is null) throw new PamelloException("Failed to select player");
        
        await RespondAsync("Player Selected", selectedPlayer.ToDiscordString, () => [player]);
    }
}
