using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueIsRandomSet : PamelloCommand
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsRandom(state, ScopeUser);
        return RequiredQueue.IsRandom;
    }
}

