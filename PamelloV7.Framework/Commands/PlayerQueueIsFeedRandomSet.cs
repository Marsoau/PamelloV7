using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueIsFeedRandomSet
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsFeedRandom(state, ScopeUser);
        return RequiredQueue.IsFeedRandom;
    }
}

