using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song.Favorites;

public partial class SongFavorites
{
    [SlashCommand("add", "Add songs to your favorites")]
    public async Task Add(
        [Summary("songs", "Songs query")] string songsQuery
    ) {
        var songs = await GetAsync<IPamelloSong>(songsQuery);
        
        var addedSongs = Command<SongFavoritesAdd>().Execute(songs);

        await RespondUpdatableAsync(message => {
            var content = string.Join("\n", addedSongs.SkipLast(addedSongs.Count - 5).Select(song => $"`[{song.Id}]` {song.Name}"));
            if (addedSongs.Count > 5) content += $"\n_... And {addedSongs.Count - 5} more_";
            
            message.Components = PamelloComponentBuilders.Info($"Added {addedSongs.Count} Songs", content).Build();
        }, [.. addedSongs]);
    }
}
