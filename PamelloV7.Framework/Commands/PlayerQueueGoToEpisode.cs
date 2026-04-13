using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueGoToEpisode
{
    public Task<IPamelloEpisode?> Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.GoToEpisode(position, ScopeUser);
    }
}

