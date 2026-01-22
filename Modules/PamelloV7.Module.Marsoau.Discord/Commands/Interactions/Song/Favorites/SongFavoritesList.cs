using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Builders;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.Favorites;

public partial class SongFavorites
{
    [SlashCommand("list", "List your favorites")]
    public async Task List() {
        await RespondUpdatableAsync(message => {
            if (Context.User.FavoriteSongs.Count == 0) {
                message.Components = PamelloComponentBuilders.Info("Favorite Songs", "You don't have any favorite songs yet").Build();
                return;
            }
            //message.Content = string.Join("\n", Context.User.FavoriteSongs.Select(song => song.ToString()));
            message.Components = PamelloComponentBuilders.Info("Favorite Songs",
                string.Join("\n", Context.User.FavoriteSongs.Select(song => $"`[{song.Id}]` {song.Name}"))
            ).Build();
        }, () => [.. Context.User.FavoriteSongs, Context.User]);
    }
}
