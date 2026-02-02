using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

public partial class Player
{
    [SlashCommand("info", "Get info about a song", runMode: RunMode.Async)]
    public async Task Create(
        [Summary("name", "Name of the player")] string name
    ) {
        var players = Services.GetRequiredService<IPamelloPlayerRepository>();
        var player = players.Create(name, User);

        await RespondUpdatableAsync(() =>
            PamelloComponentBuilders.Info("Player Created", $"{player.ToDiscordString()}").Build()
        , player);
    }
}
