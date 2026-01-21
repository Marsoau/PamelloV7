using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Builders;
using SongNameUpdated = PamelloV7.Core.Events.SongNameUpdated;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song;

public partial class Song
{
    [SlashCommand("info", "Get info about a song")]
    public async Task Info(
        [Summary("song", "Single song query")] string songQuery = "12"
    ) {
        var song = _peql.GetSingle<IPamelloSong>(songQuery, Context.User);
        if (song is null) {
            await RespondAsync("Nema tokogo");
            return;
        }

        await RespondUpdatableAsync(message => {
            message.Components = PamelloComponentBuilders.SongInfo(song, Context.User).Build();
        }, song);
    }
}
