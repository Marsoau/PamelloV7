using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlaylistClear : PamelloCommand
{
    public IPamelloPlaylist Execute(IPamelloPlaylist playlist) {
        playlist.ReplaceSongs([], ScopeUser);
        
        return playlist;
    }
}
