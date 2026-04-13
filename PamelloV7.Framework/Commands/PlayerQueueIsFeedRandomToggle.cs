using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueIsFeedRandomToggle
{
    public bool Execute() {
        RequiredQueue.SetIsFeedRandom(!RequiredQueue.IsFeedRandom, ScopeUser);
        return RequiredQueue.IsFeedRandom;
    }
}

