using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlaylistRename : PamelloCommand
{
    public IPamelloPlaylist Execute(IPamelloPlaylist playlist, string newName) {
        playlist.Name = newName;
        
        return playlist;
    }
}
