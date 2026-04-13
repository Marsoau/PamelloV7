using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerProtectionSet
{
    public bool Execute(bool state) {
        return ScopeUser.RequiredSelectedPlayer.IsProtected;
    }
}

