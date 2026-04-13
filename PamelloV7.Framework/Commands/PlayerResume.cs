using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerResume
{
    public bool Execute() {
        return RequiredSelectedPlayer.SetPause(false, ScopeUser);
    }
}
