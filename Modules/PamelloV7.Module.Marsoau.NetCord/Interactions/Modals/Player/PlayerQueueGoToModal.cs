using PamelloV7.Framework.Commands;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Player;

[DiscordModal("Queue go-to song")]

[AddShortInput("Position*", "Position")]
[AddCheckBox("Return", "Return Back")]

public partial class PlayerQueueGoToModal
{
    public void Submit() {
        Command<PlayerQueueGoTo>().Execute(Position, Return);
    }
}
