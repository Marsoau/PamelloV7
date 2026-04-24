using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song.Favorite;

[DiscordCommand("song favorite list", "View/Manage favorite songs")]
public partial class SongFavoriteList
{
    public async Task Execute(
        [UserDescription] IPamelloUser? user = null
    ) {
        user ??= ScopeUser;
        
        await RespondPageAsync(() => (user.FavoriteSongs.Count, 10), page => 
            Builder<FavoriteListBuilder>().Build(user, ESongOrPlaylist.Song, page, 10)
        , () => [ScopeUser, ..ScopeUser.FavoriteSongs]);
    }
}
