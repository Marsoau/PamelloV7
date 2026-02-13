using Discord;
using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player.Queue;

public partial class PlayerQueue
{
    [SlashCommand("skip", "Skip the current song")]
    public async Task Skip() {
        var oldSong = Context.User.RequiredSelectedPlayer.RequiredQueue.CurrentSong;
        var newSong = Command<PlayerQueueSkip>().Execute();

        await RespondUpdatableAsync(() =>
            Builder<ButtonsBuilder>().RefreshButton(Builder<SkippedSongComponent>().Component(oldSong, newSong)).Build()
        , newSong is not null ? [newSong] : []);
    }
}
