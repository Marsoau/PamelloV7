using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlaylistSongRemove : PamelloCommand
{
    public int Execute(IPamelloPlaylist playlist, IEnumerable<IPamelloSong> songs) {
        return playlist.RemoveSongs(songs, ScopeUser);
    }
}
