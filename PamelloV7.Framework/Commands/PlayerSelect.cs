using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerSelect
{
    public IPamelloPlayer? Execute(IPamelloPlayer? player) {
        return ScopeUser.SelectPlayer(player);
    }
}
