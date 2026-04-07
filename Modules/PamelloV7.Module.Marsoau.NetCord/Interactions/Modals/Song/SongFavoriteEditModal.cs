using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;

[DiscordModal("Edit favorite songs")]

[AddParagraphInput<List<IPamelloSong>>("Songs", "Songs")]

public partial class SongFavoriteEditModal
{
    public partial class Builder
    {
        public void Build() {
            Songs.Value = string.Join("\n", ScopeUser.FavoriteSongs.Select(s => s.Id));
        }
    }
    
    public void Submit() {
        ScopeUser.ReplaceFavoriteSongs(Songs);
    }
}
