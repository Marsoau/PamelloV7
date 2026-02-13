using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Enumerators;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Favorite;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song.Favorites;

public partial class SongFavorites
{
    [SlashCommand("list", "List your favorites")]
    public async Task List(
        [Summary("user", "User to view favorite songs of")] string userQuery = "current"
    ) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);
        
        await RespondUpdatablePageAsync(page =>
            Builder<FavoriteListBuilder>().Component(
                user, ESongOrPlaylist.Song, page, 10
            ).Build()
        , () => [.. Context.User.FavoriteSongs, Context.User]);
    }
}

public partial class SongFavoritesInteractions
{
    [ComponentInteraction("favorite-songs-clear:*")]
    public async Task ClearButton(string userQuery) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);
        if (user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }

        Command<SongFavoritesClear>().Execute();
        
        await ReleaseInteractionAsync();
    }
    
    [ComponentInteraction("favorite-songs-edit:*")]
    public async Task EditButton(string userQuery) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);
        if (user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }

        await RespondWithModalAsync(FavoriteEditModal.Build(user, ESongOrPlaylist.Song));
    }
}
