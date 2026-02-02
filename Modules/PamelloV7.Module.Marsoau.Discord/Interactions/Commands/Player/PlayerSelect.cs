using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

public partial class Player
{
    [SlashCommand("select", "Select a player")]
    public async Task Select(
        [Summary("player", "Player query")] string playerQuery
    ) {
        var player = await GetSingleRequiredAsync<IPamelloPlayer>(playerQuery);
        
        var selectedPlayer = Command<PlayerSelect>().Execute(player);

        await RespondUpdatableAsync(() =>
            PamelloComponentBuilders.Info("Player Selected", selectedPlayer.ToDiscordString()).Build()
        , selectedPlayer);
    }
}
