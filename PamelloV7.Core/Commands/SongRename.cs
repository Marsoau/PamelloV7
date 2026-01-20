using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class SongRename : PamelloCommand
{
    public IPamelloSong Execute(IPamelloSong song, string newName) {
        song.Name = newName;
        
        return song;
    }
}
