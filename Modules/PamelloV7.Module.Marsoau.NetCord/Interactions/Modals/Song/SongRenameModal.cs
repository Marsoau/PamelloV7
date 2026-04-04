using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;

[DiscordModal("Song Rename")]

[AddShortInput("NewName*", "Name of a song")]
[AddParagraphInput("Test", "Big input 1")]

[AddSelect("Select", "Name of select")]

public partial class SongRenameModal
{
    public partial class Builder
    {
        public void Build(IPamelloSong song) {
            NewName.Value = song.Name;
        }
    }
    
    public void Submit(IPamelloSong song) {
        Command<SongRename>().Execute(song, NewName);
    }
}
