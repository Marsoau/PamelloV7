using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueClear : PamelloCommand
{
    public void Execute() {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.Clear();
    }
}

