using Microsoft.Extensions.DependencyInjection;
using NetCord.Gateway.Voice;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Commands;
using PamelloV7.Module.Marsoau.NetCord.Config;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player.Queue;

[DiscordCommand("add", "Add songs to the queue")]
[DiscordCommand("player queue song-add", "Add songs to the queue | has shortcut /add")]
public partial class PlayerQueueSongAdd
{
    public async Task Execute(
        [SongsDescription] List<IPamelloSong> songs,
        [Description("position", "Position in queue where to insert songs")] string? position = null
    ) {
        if (NetCordConfig.Root.Commands.AutoConnectOnAddition) {
            if (SelectedPlayer is null || !SelectedPlayer.ConnectedSpeakers.Any()) {
                var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();

                if (!speakers.GetCurrent(ScopeUser).Any()) {
                    await WithLoadingAsync(
                        Command<SpeakerDiscordConnect>().Execute(Interaction.User.Id)
                    );
                }
            }
        }
        
        var addedSongs = Command<Framework.Commands.PlayerQueueSongAdd>().Execute(songs, position).ToList();

        await RespondOneOrManyAsync(
            addedSongs,
            song => [
                Builder<AddedSongsBuilder>().GetForOne(song)
            ],
            $"Added {DiscordString.Code(addedSongs.Count)} Songs"
        );
    }
}
