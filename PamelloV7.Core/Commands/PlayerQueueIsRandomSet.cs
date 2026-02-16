using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsRandomSet : PamelloCommand
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsRandom(state, ScopeUser);
        return RequiredQueue.IsRandom;
    }
}

