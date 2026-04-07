using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlaylistSongRemoveAt : PamelloCommand
{
    public IPamelloSong? Execute(IPamelloPlaylist playlist, string position) {
        return playlist.RemoveAt(position, ScopeUser);
    }
}
