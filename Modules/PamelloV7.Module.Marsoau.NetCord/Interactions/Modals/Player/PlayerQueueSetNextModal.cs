using PamelloV7.Framework.Commands;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Player;

[DiscordModal("Set next song")]

[AddShortInput("Position*", "Position")]

public partial class PlayerQueueSetNextModal
{
    public void Submit() {
        Command<PlayerQueueRequestNextPosition>().Execute(Position);
    }
}
