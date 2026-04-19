using Microsoft.Extensions.DependencyInjection;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Commands;
using PamelloV7.Module.Marsoau.NetCord.Config;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player.Queue;

[DiscordCommand("add-playlist", "Add playlists songs to the queue")]
[DiscordCommand("player queue playlist-add", "Add playlists songs to the queue | has shortcut /add-playlist")]
public partial class PlayerQueuePlaylistAdd
{
    public async Task Execute(
        [PlaylistsDescription] List<IPamelloPlaylist> playlists,
        [Description("position", "Position in queue where to insert playlists songs")] string position = "last"
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
        
        var addedPlaylists = Command<Framework.Commands.PlayerQueuePlaylistAdd>().Execute(playlists, position).ToList();

        if (playlists.Count == 1 && playlists.FirstOrDefault() is { } playlist) {
            await RespondPageAsync(page => Builder<BasicComponentsBuilder>().EntitiesList(
                $"Added {playlist.ToDiscordString()}",
                playlist.Songs,
                page
            ), () => [playlist, ..playlist.Songs]);
        }
        else {
            await RespondPageAsync(page => Builder<BasicComponentsBuilder>().EntitiesList(
                $"Added {DiscordString.Code(addedPlaylists.Count)} playlists",
                addedPlaylists,
                page
            ), () => [..addedPlaylists]);
        }
    }
}
