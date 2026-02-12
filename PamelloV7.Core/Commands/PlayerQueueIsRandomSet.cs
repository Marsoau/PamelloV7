using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsRandomSet : PamelloCommand
{
    public bool Execute(bool state) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsRandom = state;
    }
}

