using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerResume : PamelloCommand
{
    public bool Execute() {
        return RequiredSelectedPlayer.SetPause(false, ScopeUser);
    }
}
