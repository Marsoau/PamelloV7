using Discord;
using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.General;

[Map]
public class Ping : DiscordCommand
{
    [SlashCommand("ping", "Ping the bot")]
    public async Task ExecuteAsync() {
        var modalBuilder = new ModalBuilder()
            .WithTitle("Ping")
            .WithCustomId("ping-modal")
            .AddComponents(new ModalComponentBuilder()
                .WithTextDisplay("asdasdasdasd")
            );
        
        await Context.Interaction.RespondWithModalAsync(modalBuilder.Build());
        //await RespondInfo("Pong!", $"Hi {Context.User.ToDiscordString()}!");
    }
}
