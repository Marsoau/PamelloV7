using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;

[DiscordModal("Song Rename")]
public partial class SongRenameModal
{
    [AddInput("NewName")]
    public partial class SongRenameModalBuilder : DiscordModalBuilder;
    
    public async Task Submit(IPamelloSong song) {
        //Command<SongRename>().Execute(song, NetName);
    }
}
