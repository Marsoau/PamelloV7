using System.Text.Json;
using Discord.Interactions;
using PamelloV7.Framework.Converters;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Debug;

[DebugGroup]
public class DebugCommand : DiscordCommand
{
    [SlashCommand("command", "Execute a command by its path", runMode: RunMode.Async)]
    public async Task Execute(
        [Summary("path", "Command path")] string path
    ) {
        var result = await WithLoadingAsync(Command(path));
        
        var json = JsonSerializer.Serialize(result, JsonEntitiesFactory.Options);
        
        await RespondUpdatableAsync(() =>
            Builder<BasicComponentsBuilder>().Info(path, DiscordString.CodeBlock(json, "json")).Build()
        );
    }
}
