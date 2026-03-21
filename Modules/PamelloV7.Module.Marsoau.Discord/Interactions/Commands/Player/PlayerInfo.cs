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
public class PlayerInfoCommand : DiscordCommand
{
    [SlashCommand("info", "Info about selected player")]
    public async Task Info() {
        await RespondUpdatableAsync(() => 
            Builder<PlayerInfoBuilder>().Component().Build(),
        () => [ScopeUser, ScopeUser.SelectedPlayer, ScopeUser.SelectedPlayer?.Queue?.CurrentSong]);
    }
}
