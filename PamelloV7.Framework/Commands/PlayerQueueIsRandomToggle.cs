using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueIsRandomToggle : PamelloCommand
{
    public bool Execute() {
        RequiredQueue.SetIsRandom(!RequiredQueue.IsRandom, ScopeUser);
        return RequiredQueue.IsRandom;
    }
}

