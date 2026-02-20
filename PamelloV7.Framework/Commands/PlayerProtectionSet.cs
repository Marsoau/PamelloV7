using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerProtectionSet : PamelloCommand
{
    public bool Execute(bool state) {
        return ScopeUser.RequiredSelectedPlayer.IsProtected;
    }
}

