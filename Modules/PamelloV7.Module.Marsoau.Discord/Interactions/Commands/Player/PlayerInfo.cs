using Discord.Audio;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

public partial class Player
{
    [SlashCommand("info", "Info about selected player")]
    public async Task Info() {
        await RespondUpdatableAsync(() => 
            Builder<PlayerInfoBuilder>().Component().Build(),
        () => [ScopeUser, ScopeUser.SelectedPlayer, ScopeUser.SelectedPlayer?.Queue?.CurrentSong]);
    }
}
