using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song;

public partial class Song
{
    [SlashCommand("rename", "Rename a song")]
    public async Task Rename(
        [Summary("name", "New name for the song")] string newName,
        [Summary("song", "Single song query")] string songQuery = "current"
    ) {
        var song = await GetSingleAsync<IPamelloSong>(songQuery);
        if (song is null) {
            await RespondAsync("Nema tokogo");
            return;
        }
        
        Command<SongRename>().Execute(song, newName);

        await RespondUpdatableAsync((message) => {
            message.Components = PamelloComponentBuilders.Info("Song Renamed", song.ToDiscordString()).Build();
        }, song);
    }
}
