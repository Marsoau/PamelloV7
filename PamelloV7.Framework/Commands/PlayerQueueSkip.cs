using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueSkip : PamelloCommand
{
    public IPamelloSong? Execute() {
        var currentSong = RequiredQueue.CurrentSong;
        RequiredQueue.GoToNextSong(ScopeUser);
        return currentSong;
    }
}
