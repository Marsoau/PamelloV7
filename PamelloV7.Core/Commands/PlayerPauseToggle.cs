using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerPauseToggle : PamelloCommand
{
    public bool Execute() {
        return ScopeUser.RequiredSelectedPlayer.IsPaused = !ScopeUser.RequiredSelectedPlayer.IsPaused;
    }
}
