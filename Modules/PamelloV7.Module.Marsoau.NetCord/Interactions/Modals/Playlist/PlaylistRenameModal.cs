using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Playlist;

[DiscordModal("Rename playlist")]

[AddShortInput("Name*", "New Name")]

public partial class PlaylistRenameModal
{
    public partial class Builder
    {
        public void Build(IPamelloPlaylist playlist) {
            Name.Value = playlist.Name;
        }
    }
    
    public void Submit(IPamelloPlaylist playlist) {
        Command<PlaylistRename>().Execute(playlist, Name);
    }
}
