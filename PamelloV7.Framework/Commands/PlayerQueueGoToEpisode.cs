using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueGoToEpisode : PamelloCommand
{
    public Task<IPamelloEpisode?> Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.GoToEpisode(position, ScopeUser);
    }
}

