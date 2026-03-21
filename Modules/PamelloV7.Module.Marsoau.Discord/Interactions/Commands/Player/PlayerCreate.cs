using Discord.Audio;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

[PlayerGroup]
public class PlayerCreateCommand : DiscordCommand
{
    [SlashCommand("create", "Create a new player", runMode: RunMode.Async)]
    public async Task Create(
        [Summary("name", "Name of the player")] string name
    ) {
        var player = Command<PlayerCreate>().Execute(name);

        await RespondUpdatableAsync(() =>
            Builder<BasicComponentsBuilder>().Info("Player Created & Selected", $"{player.ToDiscordString()} ({player.Queue?.Count})").Build()
        , player);
    }
}
