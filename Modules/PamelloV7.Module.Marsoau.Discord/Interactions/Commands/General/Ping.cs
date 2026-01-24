using Discord;
using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.General;

[Map]
public class Ping : DiscordCommand
{
    [SlashCommand("ping", "Ping the bot")]
    public async Task ExecuteAsync() {
        await RespondUpdatableAsync(message => {
            message.Components = PamelloComponentBuilders.Info("Pong!", $"Hi {Context.User.ToDiscordString()}!").Build();
        }, Context.User);
    }
}
