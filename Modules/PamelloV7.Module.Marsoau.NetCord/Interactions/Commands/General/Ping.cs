using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;

[DiscordCommand("ping", "Ping the bot")]
[DiscordCommand("test grouped alias", "Ping the bot alias")]
public partial class Ping
{
    public async Task Execute() {
        await RespondAsync("Pong!", $"Hi {ScopeUser.ToDiscordString()}!");
    }
}
