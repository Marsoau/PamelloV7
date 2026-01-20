using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song;

public partial class Song
{
    [SlashCommand("rename", "Rename a song")]
    public async Task RenameAsync(
        [Summary("name", "New name for the song")] string newName,
        [Summary("song", "Single song query")] string songQuery = "current"
    ) {
        var peql = Services.GetRequiredService<IEntityQueryService>();
        
        var song = peql.GetSingle<IPamelloSong>(songQuery, Context.User);
        if (song is null) {
            await RespondAsync("Nema tokogo");
            return;
        }
        
        Command<SongRename>().Execute(song, newName);

        await RespondUpdatableAsync((message) => {
            message.Content = $"Renamed `{song}`";
        }, song);
    }
}
