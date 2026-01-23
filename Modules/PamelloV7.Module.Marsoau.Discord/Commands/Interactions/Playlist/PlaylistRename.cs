using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Playlist;

public partial class Playlist
{
    [SlashCommand("rename", "Rename a playlist")]
    public async Task Rename(
        [Summary("name", "New name for the playlist")] string newName,
        [Summary("playlist", "Single playlist to rename")] string playlistQuery
    ) {
        var playlist = _peql.GetSingle<IPamelloPlaylist>(playlistQuery, Context.User);
        if (playlist is null) {
            await RespondAsync("Nema tokogo");
            return;
        }
        
        Command<PlaylistRename>().Execute(playlist, newName);

        await RespondUpdatableAsync((message) => {
            message.Components = PamelloComponentBuilders.RefreshButton(
                PamelloComponentBuilders.Info("Playlist Renamed", playlist.ToDiscordString())
            ).Build();
        }, playlist);
    }
}
