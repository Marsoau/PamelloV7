using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueSongRemoveRange
{
    public IEnumerable<IPamelloSong> Execute(string fromPosition, string toPosition) {
        return RequiredQueue.RemoveSongsRange(fromPosition, toPosition, ScopeUser);
    }
}
