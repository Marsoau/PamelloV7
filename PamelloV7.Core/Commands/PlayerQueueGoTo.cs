using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlayerQueueGoTo : PamelloCommand
{
    public IPamelloSong? Execute(string position, bool returnBack = false) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.GoToSong(position, returnBack);
    }
}

