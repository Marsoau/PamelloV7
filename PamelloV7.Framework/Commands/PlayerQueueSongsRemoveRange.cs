using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueSongsRemoveRange : PamelloCommand
{
    public IEnumerable<IPamelloSong> Execute(string fromPosition, string toPosition) {
        return RequiredQueue.RemoveSongsRange(fromPosition, toPosition, ScopeUser);
    }
}
