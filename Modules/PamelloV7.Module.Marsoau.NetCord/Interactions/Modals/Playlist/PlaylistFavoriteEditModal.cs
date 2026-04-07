using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Playlist;

[DiscordModal("Edit favorite songs")]

[AddParagraphInput<List<IPamelloPlaylist>>("Playlists", "Playlists")]

public partial class PlaylistFavoriteEditModal
{
    public partial class Builder
    {
        public void Build() {
            Playlists.Value = string.Join("\n", ScopeUser.FavoritePlaylists.Select(s => s.Id));
        }
    }
    
    public void Submit() {
        ScopeUser.ReplaceFavoritePlaylists(Playlists);
    }
}
