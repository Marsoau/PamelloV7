using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Playlist;

[DiscordModal("Transfer playlist ownership")]

[AddPamelloUserSelect("NewOwner*", "New Owner")]

public partial class PlaylistTransferOwnershipModal
{
    public void Submit(IPamelloPlaylist playlist) {
        Command<PlaylistTransferOwnership>().Execute(playlist, NewOwner);
    }
}
