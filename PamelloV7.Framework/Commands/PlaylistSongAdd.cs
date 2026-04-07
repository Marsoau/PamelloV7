using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlaylistSongAdd : PamelloCommand
{
    public void Execute(IPamelloPlaylist playlist, IEnumerable<IPamelloSong> songs, int? position = null) {
        playlist.AddSongs(songs, ScopeUser, position);
    }
}

