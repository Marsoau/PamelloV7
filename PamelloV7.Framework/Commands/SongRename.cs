using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongRename
{
    public IPamelloSong Execute(IPamelloSong song, string newName) {
        song.SetName(newName, ScopeUser);
        return song;
    }
}
