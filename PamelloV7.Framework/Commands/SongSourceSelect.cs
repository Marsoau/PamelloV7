using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class SongSourceSelect : PamelloCommand
{
    public void Execute(IPamelloSong song, int index) {
        song.SelectSource(index, ScopeUser);
    }
}
