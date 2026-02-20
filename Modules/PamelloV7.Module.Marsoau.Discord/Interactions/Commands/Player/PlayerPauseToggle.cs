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
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

public partial class Player
{
    [SlashCommand("pause-toggle", "Toggle the current pause state of the player (resume | pause)")]
    public async Task PauseToggle() {
        Command<PlayerPauseToggle>().Execute();
        
        await RespondUpdatableAsync(() => 
            Builder<PlayerPauseToggleBuilder>().Component().Build(),
        () => ScopeUser.SelectedPlayer is not null ? [ScopeUser, ScopeUser.SelectedPlayer] : [ScopeUser]);
    }
}
