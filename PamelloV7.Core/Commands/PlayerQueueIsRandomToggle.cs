using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsRandomToggle : PamelloCommand
{
    public bool Execute() {
        RequiredQueue.SetIsRandom(!RequiredQueue.IsRandom, ScopeUser);
        return RequiredQueue.IsRandom;
    }
}

