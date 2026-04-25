using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlaylistIsProtectedToggle
{
    public bool Execute(IPamelloPlaylist playlist) {
        return playlist.SetIsProtected(!playlist.IsProtected, ScopeUser);
    }
}
