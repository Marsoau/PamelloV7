using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Exceptions;

namespace PamelloV7.Framework.Commands;

public class SongInfoReset : PamelloCommand
{
    public async Task<IPamelloSong> Execute(IPamelloSong song) {
        if (song.SelectedSource is null) return song;
        
        await song.SelectedSource.UpdateInfo();
        song.SelectedSource.ResetSongInfo(ScopeUser);
        
        return song;
    }
}
