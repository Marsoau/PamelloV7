using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlaylistTransferOwnership
{
    public void Execute(IPamelloPlaylist playlist, IPamelloUser newOwner) {
        playlist.TransferOwnership(newOwner, ScopeUser);
    }
}
