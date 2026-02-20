using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlaylistRename : PamelloCommand
{
    public IPamelloPlaylist Execute(IPamelloPlaylist playlist, string newName) {
        playlist.SetName(newName, ScopeUser);
        
        return playlist;
    }
}
