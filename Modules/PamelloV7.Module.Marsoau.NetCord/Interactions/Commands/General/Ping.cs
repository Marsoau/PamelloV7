using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;

[DiscordCommand("ping", "Ping the bot")]
[DiscordCommand("execution clap", "clap")]
[DiscordCommand("ado film red", "red")]
public class Ping : DiscordCommand
{
    public async Task Execute() {
        await RespondAsync("Pong!", $"Hi {ScopeUser}!");
    }
}
