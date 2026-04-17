using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueSongsReplace
{
    public void Execute(IEnumerable<IPamelloSong> songs) {
        RequiredQueue.ReplaceSongs(songs, ScopeUser);
    }
}
