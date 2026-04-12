using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Debug;

[DiscordCommand("debug display-error", "Display an error")]
public partial class DebugDisplayError
{
    public async Task Execute(string message, string header = "Error") {
        await RespondAsync(() =>
            Builder<ErrorMessageBuilder>().Build(header, message, 5)
        , () => [], 5);
    }
}
