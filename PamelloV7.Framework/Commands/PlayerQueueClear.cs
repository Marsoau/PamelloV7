using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueClear : PamelloCommand
{
    public void Execute() {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.Clear(ScopeUser);
    }
}

