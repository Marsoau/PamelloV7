using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueClear
{
    public void Execute() {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.Clear(ScopeUser);
    }
}

