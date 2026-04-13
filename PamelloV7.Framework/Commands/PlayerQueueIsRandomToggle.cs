using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueIsRandomToggle
{
    public bool Execute() {
        RequiredQueue.SetIsRandom(!RequiredQueue.IsRandom, ScopeUser);
        return RequiredQueue.IsRandom;
    }
}

