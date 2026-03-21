using Discord.Interactions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song.Favorites;

[SongFavoriteGroup]
public class SongFavoritesAddCommand : DiscordCommand
{
    [SlashCommand("add", "Add songs to your favorites")]
    public async Task Add(
        [Summary("songs", "Songs query")] string songsQuery = "current"
    ) {
        var songs = await GetAsync<IPamelloSong>(songsQuery);
        
        var addedSongs = Command<SongFavoritesAdd>().Execute(songs);

        await RespondUpdatablePageAsync(page => {
            var content = string.Join("\n", addedSongs.SkipLast(addedSongs.Count - 5).Select(song => $"`[{song.Id}]` {song.Name}"));
            if (addedSongs.Count > 5) content += $"\n_... And {addedSongs.Count - 5} more_";

            return Builder<BasicComponentsBuilder>().EntitiesList($"Added {addedSongs.Count} Songs", addedSongs, page, noResultsMessage: $"No songs added to favorite by query \"{songsQuery}\"").Build();
        }, () => [.. addedSongs]);
    }
}
