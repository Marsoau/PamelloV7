using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song;

[DiscordCommand("song rename", "Rename a song")]
public partial class SongRename
{
    public async Task Execute(
        [Description("new-name", "New name of the song")] string newName,
        [SongDescription] [DefaultQuery("current")] IPamelloSong song
    ) {
        Command<Framework.Commands.SongRename>().Execute(song, newName);

        await RespondAsync("Song Renamed", song.ToDiscordString, () => [song]);
    }
}
