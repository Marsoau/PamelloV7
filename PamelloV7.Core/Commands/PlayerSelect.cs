using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlayerSelect : PamelloCommand
{
    public IPamelloPlayer Execute(IPamelloPlayer player) {
        return ScopeUser.SelectedPlayer = player;
    }
}
