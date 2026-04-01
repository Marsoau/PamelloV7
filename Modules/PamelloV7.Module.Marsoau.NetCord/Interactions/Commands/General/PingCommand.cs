using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;

[DiscordCommand("ping", "Ping the bot")]
[DiscordCommand("execution clap", "clap")]
[DiscordCommand("ado film red", "red")]
public class PingCommand : DiscordCommand
{
    public async Task Execute(int num, bool yes, string opt = "aa") {
        await Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Content = $"Num: {num}; Yes: {yes}; Opt: {opt}",
            Flags = MessageFlags.Ephemeral
        }));
    }
}
