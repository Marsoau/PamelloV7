using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlaylistSongMove
{
    public void Execute(IPamelloPlaylist playlist, string fromPosition, string toPosition) {
        playlist.MoveSong(fromPosition, toPosition, ScopeUser);
    }
}

