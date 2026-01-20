using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.General;

public class Ping : DiscordCommand
{
    [SlashCommand("ping", "Ping the bot")]
    public async Task ExecuteAsync() {
        await RespondAsync($"Hi {Context.User}");
    }
}
