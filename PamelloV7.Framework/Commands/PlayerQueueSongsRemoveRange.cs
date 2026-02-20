using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueSongsRemoveRange : PamelloCommand
{
    public int Execute(string fromPosition, string toPosition) {
        return RequiredQueue.RemoveSongsRange(fromPosition, toPosition, ScopeUser);
    }
}
