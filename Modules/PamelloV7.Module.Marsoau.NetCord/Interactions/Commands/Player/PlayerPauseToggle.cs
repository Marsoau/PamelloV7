using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player;

[DiscordCommand("player pause-toggle", "Toggle player pause state")]
public partial class PlayerPauseToggle
{
    public async Task Execute() {
        await RespondAsync(() =>
            Builder<PlayerPauseToggleBuilder>().Container()
        , () => [ScopeUser, ScopeUser.SelectedPlayer]);
    }
}
