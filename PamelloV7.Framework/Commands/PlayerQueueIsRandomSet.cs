using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueIsRandomSet
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsRandom(state, ScopeUser);
        return RequiredQueue.IsRandom;
    }
}

