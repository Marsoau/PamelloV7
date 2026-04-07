using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Playlist;

[DiscordModal("Edit playlist")]

[AddParagraphInput<List<IPamelloSong>>("Songs", "Playlists")]

public partial class PlaylistEditModal
{
    public partial class Builder
    {
        public void Build(IPamelloPlaylist playlist) {
            Songs.Value = string.Join("\n", playlist.Songs.Select(s => s.Id));
        }
    }
    
    public void Submit(IPamelloPlaylist playlist) {
        playlist.ReplaceSongs(Songs, ScopeUser);
    }
}
