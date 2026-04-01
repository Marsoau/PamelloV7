using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;

[DiscordCommand("ping", "Ping the bot")]
public class PingCommand : DiscordCommand
{
    public async Task Execute() {
        await Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Content = Interaction.Data.Id.ToString(),
            Flags = MessageFlags.Ephemeral
        }));
    }
}
