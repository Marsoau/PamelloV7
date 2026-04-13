using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueIsReversedToggle
{
    public bool Execute() {
        RequiredQueue.SetIsReversed(!RequiredQueue.IsReversed, ScopeUser);
        return RequiredQueue.IsReversed;
    }
}

