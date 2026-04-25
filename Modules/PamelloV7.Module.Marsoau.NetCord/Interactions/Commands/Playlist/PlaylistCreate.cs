using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Playlist;

[DiscordCommand("playlist create", "Create a playlist")]
public partial class PlaylistCreate
{
    public async Task Execute(
        [Description("name", "Name of the playlist")] string name,
        [Description("songs", "Songs to fill the playlist with")] [DefaultQuery("")] List<IPamelloSong> songs
    ) {
        var playlist = Command<Framework.Commands.PlaylistCreate>().Execute(name);
        playlist.AddSongs(songs, ScopeUser);

        await RespondAsync("Playlist Created", () => playlist);
    }
}
