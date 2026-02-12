using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerProtectionSet : PamelloCommand
{
    public bool Execute(bool state) {
        return ScopeUser.RequiredSelectedPlayer.IsProtected = state;
    }
}

