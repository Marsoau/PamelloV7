using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.Song.Favorite;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.Favorites;

public partial class SongFavorites
{
    [SlashCommand("list", "List your favorites")]
    public async Task List(
        [Summary("user", "User to view favorite songs of")] string userQuery = "1"
    ) {
        await RespondUpdatablePageAsync((message, page) => {
            message.Components = PamelloComponentBuilders.FavoriteList(
                _peql.GetSingleRequired<IPamelloUser>(userQuery, Context.User), Context.User, page, 3
            ).Build();
        }, () => [.. Context.User.FavoriteSongs, Context.User]);
    }
    
}

public partial class SongFavoritesInteractions
{
    [ComponentInteraction("favorite-list-clear:*")]
    public async Task ClearButton(string userQuery) {
        var user = _peql.GetSingleRequired<IPamelloUser>(userQuery, Context.User);
        if (user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }

        Command<SongFavoritesClear>().Execute();
        
        await ReleaseInteractionAsync();
    }
    
    [ComponentInteraction("favorite-list-edit:*")]
    public async Task EditButton(string userQuery) {
        var user = _peql.GetSingleRequired<IPamelloUser>(userQuery, Context.User);
        if (user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }

        await RespondWithModalAsync(SongFavoriteEditModal.Build(user));
    }
}
