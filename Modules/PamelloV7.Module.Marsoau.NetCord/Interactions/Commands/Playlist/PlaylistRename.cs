using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Playlist;

[DiscordCommand("playlist rename", "Rename a playlist")]
public partial class PlaylistRename
{
    public async Task Execute(
        [PlaylistDescription] IPamelloPlaylist playlist,
        [Description("new-name", "New name of the playlist")] string newName
    ) {
        Command<Framework.Commands.PlaylistRename>().Execute(playlist, newName);
        
        await RespondAsync("Playlist Renamed", playlist.ToDiscordString, () => [playlist]);
    }
}
