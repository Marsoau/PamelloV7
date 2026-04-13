using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class SongEpisodesClear : PamelloCommand
{
    public void Execute(IPamelloSong song) {
        song.RemoveAllEpisodes(ScopeUser);
    }
}
