using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueIsNoLeftoversToggle
{
    public bool Execute() {
        RequiredQueue.SetIsNoLeftovers(!RequiredQueue.IsNoLeftovers, ScopeUser);
        return RequiredQueue.IsNoLeftovers;
    }
}

