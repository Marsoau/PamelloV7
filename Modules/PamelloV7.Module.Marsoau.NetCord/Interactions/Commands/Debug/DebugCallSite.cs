using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Debug;

[DiscordCommand("debug call-site", "Debug callsite")]
public partial class DebugCallSite
{
    public async Task Execute() {
        await RespondAsync(() => [
            new ActionRowProperties().AddComponents(
                Button("Action", ButtonStyle.Primary, () => { }),
                ModalButton<DebugCallSiteModal>("Modal", ButtonStyle.Success)
            ),
            new ComponentSeparatorProperties(),
            new ActionRowProperties().AddComponents(
                Button<RefreshButton>()
            )
        ]);
    }
}

[DiscordModal("Debug CallSite")]
public partial class DebugCallSiteModal
{
    public void Submit() {
        Output.Write("Debug CallSite Submitted");
    }
}