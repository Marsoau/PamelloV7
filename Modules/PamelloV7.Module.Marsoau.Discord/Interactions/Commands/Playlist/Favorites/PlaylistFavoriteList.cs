using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Enumerators;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Favorite;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Playlist.Favorites;

public partial class PlaylistFavorite
{
    [SlashCommand("list", "List your favorite playlists")]
    public async Task List(
        [Summary("user", "User to view favorite playlists of")] string userQuery = "1"
    ) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);
        
        await RespondUpdatablePageAsync(page =>
            Builder<FavoriteListBuilder>().Component(
                user, ESongOrPlaylist.Playlist, page, 10
            ).Build()
        , () => [.. Context.User.FavoritePlaylists, Context.User]);
    }
}

public partial class PlaylistFavoriteInteractions
{
    [ComponentInteraction("favorite-playlists-clear:*")]
    public async Task ClearButton(string userQuery) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);
        if (user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }

        Command<PlaylistFavoriteClear>().Execute();
        
        await ReleaseInteractionAsync();
    }
    
    [ComponentInteraction("favorite-playlists-edit:*")]
    public async Task EditButton(string userQuery) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);
        if (user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }

        await RespondWithModalAsync(FavoriteEditModal.Build(user, ESongOrPlaylist.Playlist));
    }
}
