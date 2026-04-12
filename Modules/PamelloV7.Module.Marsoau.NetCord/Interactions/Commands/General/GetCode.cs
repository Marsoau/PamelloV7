using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Services;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;

[DiscordCommand("get-code", "Get your authorization code")]
public partial class GetCode
{
    public async Task Execute() {
        var codes = Services.GetRequiredService<ICodeAuthorizationService>();
        
        await RespondAsync("Your code", DiscordString.Secret(codes.GetCode(ScopeUser)));
    }
}
