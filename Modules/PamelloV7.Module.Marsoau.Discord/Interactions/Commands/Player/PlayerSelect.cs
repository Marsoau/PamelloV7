using Discord.Interactions;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

[PlayerGroup]
public partial class PlayerSelectCommand : DiscordCommand
{
    [SlashCommand("select", "Select a player")]
    public async Task Select(
        [Summary("player", "Player query")] string playerQuery
    ) {
        var player = await GetSingleRequiredAsync<IPamelloPlayer>(playerQuery);
        
        var selectedPlayer = Command<PlayerSelect>().Execute(player);
        if (selectedPlayer is null) {
            throw new PamelloException("Failed to select player");
        }

        await RespondUpdatableAsync(() =>
            Builder<BasicComponentsBuilder>().Info("Player Selected", selectedPlayer.ToDiscordString()).Build()
        , selectedPlayer);
    }
}
