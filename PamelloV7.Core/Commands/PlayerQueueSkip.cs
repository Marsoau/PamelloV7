using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlayerQueueSkip : PamelloCommand
{
    public IPamelloSong? Execute() {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.GoToNextSong();
    }
}
