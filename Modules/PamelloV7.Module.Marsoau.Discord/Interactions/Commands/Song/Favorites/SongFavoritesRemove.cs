using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song.Favorites;

public partial class SongFavorites
{
    [SlashCommand("remove", "Remove songs from your favorites")]
    public async Task Remove(
        [Summary("songs", "Songs query")] string songsQuery
    ) {
        var songs = await GetAsync<IPamelloSong>(songsQuery);
        
        var removedSongs = Command<SongFavoritesRemove>().Execute(songs);

        await RespondUpdatablePageAsync((message, page) => {
            var content = string.Join("\n", removedSongs.SkipLast(removedSongs.Count - 5).Select(song => $"`[{song.Id}]` {song.Name}"));
            if (removedSongs.Count > 5) content += $"\n_... And {removedSongs.Count - 5} more_";

            message.Components = PamelloComponentBuilders.EntitiesList($"Added {removedSongs.Count} Songs", removedSongs, page, noResultsMessage: $"No songs added to favorite by query \"{songsQuery}\"").Build();
        }, () => [.. removedSongs]);
    }
}
