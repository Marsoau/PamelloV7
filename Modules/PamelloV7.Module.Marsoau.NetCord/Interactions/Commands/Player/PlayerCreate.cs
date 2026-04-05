using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player;

[DiscordCommand("player create", "Create a player")]
public partial class PlayerCreate
{
    public async Task Execute(
        [Description("name", "Name of the player")] string name
    ) {
        var player = Command<Framework.Commands.PlayerCreate>().Execute(name);

        await RespondAsync("Player Created & Selected", player.ToDiscordString, () => [player]);
    }
}
