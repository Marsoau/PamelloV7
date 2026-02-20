using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueGoTo : PamelloCommand
{
    public IPamelloSong? Execute(string position, bool returnBack = false) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.GoToSong(position, ScopeUser, returnBack);
    }
}

