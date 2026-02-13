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
    [SlashCommand("pause-toggle", "Toggle the current pause state of the player (resume | pause)")]
    public async Task PauseToggle() {
        Command<PlayerPauseToggle>().Execute();
        
        await RespondUpdatableAsync(() => 
            Builder<PlayerPauseToggleBuilder>().Component().Build(),
        () => Context.User.SelectedPlayer is not null ? [Context.User.SelectedPlayer] : []);
    }
}
