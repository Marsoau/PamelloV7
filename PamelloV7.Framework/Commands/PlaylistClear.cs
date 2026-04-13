using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlaylistClear
{
    public IPamelloPlaylist Execute(IPamelloPlaylist playlist) {
        playlist.ReplaceSongs([], ScopeUser);
        
        return playlist;
    }
}
