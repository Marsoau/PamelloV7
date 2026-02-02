using Discord;
using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Builders.Components;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player.Queue;

public partial class PlayerQueue
{
    [SlashCommand("skip", "Skip the current song")]
    public async Task Skip() {
        var oldSong = Context.User.RequiredSelectedPlayer.RequiredQueue.CurrentSong;
        var newSong = Command<PlayerQueueSkip>().Execute();

        await RespondUpdatableAsync(() =>
            PamelloComponentBuilders.RefreshButton(SkippedSongComponent.Get(oldSong, newSong)).Build()
            /*
            PamelloComponentBuilders.Info("Skip",
                (oldSong is null ?
                    "Nothing was skipped" :
                    $"Skipped {oldSong.ToDiscordString()}"
                ) + (newSong is null ? "" : $"\nNow playing {newSong.ToDiscordString()}")
            ).Build()
            */
        , newSong is not null ? [newSong] : []);
    }
}
