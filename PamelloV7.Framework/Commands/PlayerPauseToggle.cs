using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerPauseToggle : PamelloCommand
{
    public bool Execute() {
        return RequiredSelectedPlayer.SetPause(!RequiredSelectedPlayer.IsPaused, ScopeUser);
    }
}
