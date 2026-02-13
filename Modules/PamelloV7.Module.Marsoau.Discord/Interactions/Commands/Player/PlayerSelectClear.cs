using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

public partial class Player
{
    [SlashCommand("select-clear", "Clear your current player selection")]
    public async Task SelectClear() {
        if (ScopeUser.SelectedPlayer is null) {
            await RespondUpdatableAsync(() =>
                Builder<BasicComponentsBuilder>().Info("No Selected Player").Build()
            , () => []);
            return;
        }
        
        Command<PlayerSelect>().Execute(null);
        
        await RespondUpdatableAsync(() =>
            Builder<BasicComponentsBuilder>().Info("Player Selection Cleared").Build()
        , () => []);
    }
}
