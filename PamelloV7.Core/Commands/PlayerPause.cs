using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerPause : PamelloCommand
{
    public bool Execute() {
        return ScopeUser.RequiredSelectedPlayer.IsPaused = true;
    }
}
