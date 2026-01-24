using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Core.Commands;

public class SongInfoReset : PamelloCommand
{
    public async Task<IPamelloSong> Execute(IPamelloSong song, int index) {
        if (index < 0 || index >= song.Sources.Count) throw new PamelloException("No source can be found");
        
        var source = song.Sources.ElementAtOrDefault(index);
        if (source is null) throw new PamelloException($"No source found by index `{index}` in song {song}");

        await source.UpdateInfo();
        source.SetInfoToSong();
        
        return song;
    }
}
