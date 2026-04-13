using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlaylistSongRemove
{
    public int Execute(IPamelloPlaylist playlist, IEnumerable<IPamelloSong> songs) {
        return playlist.RemoveSongs(songs, ScopeUser);
    }
}
