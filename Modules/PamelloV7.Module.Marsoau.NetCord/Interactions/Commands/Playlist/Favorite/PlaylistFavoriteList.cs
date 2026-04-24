using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song.Favorite;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Playlist.Favorite;

[DiscordCommand("playlist favorite list", "View/Manage favorite playlists")]
public partial class PlaylistFavoriteList
{
    public async Task Execute(
        [UserDescription] IPamelloUser? user = null
    ) {
        user ??= ScopeUser;
        
        await RespondPageAsync(() => (user.FavoritePlaylists.Count, 10), page => 
            Builder<FavoriteListBuilder>().Build(user, ESongOrPlaylist.Playlist, page, 10)
        , () => [ScopeUser, ..ScopeUser.FavoritePlaylists]);
    }
}
