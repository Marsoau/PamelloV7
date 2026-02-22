using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerPause : PamelloCommand
{
    public bool Execute() {
        return RequiredSelectedPlayer.SetPause(true, ScopeUser);
    }
}
