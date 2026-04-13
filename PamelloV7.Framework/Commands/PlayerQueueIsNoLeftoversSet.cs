using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueIsNoLeftoversSet
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsNoLeftovers(state, ScopeUser);
        return RequiredQueue.IsNoLeftovers;
    }
}

