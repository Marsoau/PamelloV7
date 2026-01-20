using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using SongNameUpdated = PamelloV7.Core.Events.SongNameUpdated;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song;

public partial class Song
{
    [SlashCommand("info", "Get info about a song")]
    public async Task Info(
        [Summary("song", "Single song query")] string songQuery = "current"
    ) {
        var peql = Services.GetRequiredService<IEntityQueryService>();
        
        var song = peql.GetSingle<IPamelloSong>(songQuery, Context.User);
        if (song is null) {
            await RespondAsync("Nema tokogo");
            return;
        }

        await RespondUpdatableAsync(message => {
            message.Content = $"`{song}`";
        }, song);
    }
}
