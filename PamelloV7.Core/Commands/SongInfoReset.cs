using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class SongInfoReset : PamelloCommand
{
    public List<IPamelloSong> Execute(List<IPamelloSong> songs, int sourcePos) {
        songs.ForEach(song => song.Sources.ElementAtOrDefault(sourcePos)?.SetInfoToSong());
        
        return songs;
    }
}
