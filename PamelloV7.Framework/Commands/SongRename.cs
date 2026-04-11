using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class SongRename : PamelloCommand
{
    public IPamelloSong Execute(IPamelloSong song, string newName) {
        song.SetName(newName, ScopeUser);
        return song;
    }
}
