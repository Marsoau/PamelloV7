using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.Favorite;
using PamelloV7.Module.Marsoau.Discord.Enumerators;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Playlist.Favorites;

public partial class PlaylistFavorite
{
    [SlashCommand("list", "List your favorite playlists")]
    public async Task List(
        [Summary("user", "User to view favorite playlists of")] string userQuery = "1"
    ) {
        await RespondUpdatablePageAsync((message, page) => {
            message.Components = PamelloComponentBuilders.FavoriteList(
                _peql.GetSingleRequired<IPamelloUser>(userQuery, Context.User), ESongOrPlaylist.Playlist, Context.User, page, 10
            ).Build();
        }, () => [.. Context.User.FavoritePlaylists, Context.User]);
    }
}

public partial class PlaylistFavoriteInteractions
{
    [ComponentInteraction("favorite-playlists-clear:*")]
    public async Task ClearButton(string userQuery) {
        var user = _peql.GetSingleRequired<IPamelloUser>(userQuery, Context.User);
        if (user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }

        Command<PlaylistFavoriteClear>().Execute();
        
        await ReleaseInteractionAsync();
    }
    
    [ComponentInteraction("favorite-playlists-edit:*")]
    public async Task EditButton(string userQuery) {
        var user = _peql.GetSingleRequired<IPamelloUser>(userQuery, Context.User);
        if (user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }

        await RespondWithModalAsync(FavoriteEditModal.Build(user, ESongOrPlaylist.Playlist));
    }
}
