using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueIsReversedSet
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsReversed(state, ScopeUser);
        return RequiredQueue.IsReversed;
    }
}

