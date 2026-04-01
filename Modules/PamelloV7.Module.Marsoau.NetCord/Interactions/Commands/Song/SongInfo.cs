using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song;

[DiscordCommand("song info", "Get info about a song")]
public class SongInfo : DiscordCommand
{
    public async Task Execute(
        [SongDescription] IPamelloSong song
    ) {
        await RespondAsync($"Song: {song}", () => [song]);
    }
}
