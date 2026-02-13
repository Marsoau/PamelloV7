using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlayerQueueGoToEpisode : PamelloCommand
{
    public Task<IPamelloEpisode?> Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.GoToEpisode(position);
    }
}

