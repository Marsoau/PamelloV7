using Discord.Interactions;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.SongFavorites;

public partial class Song
{
    public partial class SongFavorites
    {
        [SlashCommand("list", "List your favorites")]
        public async Task List() {
            await RespondUpdatableAsync(message => {
                if (Context.User.FavoriteSongs.Count == 0) {
                    message.Content = "No favorite songs";
                    return;
                }
                message.Content = string.Join("\n", Context.User.FavoriteSongs.Select(song => song.ToString()));
            }, () => [.. Context.User.FavoriteSongs, Context.User]);
        }
    }
}
