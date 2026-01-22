using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Core.Commands;

public class SongInfoReset : PamelloCommand
{
    public IPamelloSong Execute(IPamelloSong song, string? platformKey = null) {
        if ((platformKey?.Length ?? 0) == 0) platformKey = song.Sources.FirstOrDefault()?.PK.ToString();
        if (platformKey is null) throw new PamelloException("No source can be found");
        
        var source = song.Sources.First(s => s.PK.ToString() == platformKey);
        if (source is null) throw new PamelloException($"No source found by platform key `{platformKey}`");

        source.SetInfoToSong();
        
        return song;
    }
}
