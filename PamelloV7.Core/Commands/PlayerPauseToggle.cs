using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerPauseToggle : PamelloCommand
{
    public bool Execute() {
        return RequiredSelectedPlayer.SetPause(!RequiredSelectedPlayer.IsPaused, ScopeUser);
    }
}
