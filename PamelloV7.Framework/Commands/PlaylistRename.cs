using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlaylistRename
{
    public IPamelloPlaylist Execute(IPamelloPlaylist playlist, string newName) {
        playlist.SetName(newName, ScopeUser);
        
        return playlist;
    }
}
