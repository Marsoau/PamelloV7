using PamelloV7.Framework.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Debug;

[DiscordCommand("debug get-token", "Get your PamelloUser token")]
public partial class DebugGetToken
{
    public async Task Execute() {
        await RespondAsync("Your Token", DiscordString.Secret(ScopeUser.Token));
    }
}
