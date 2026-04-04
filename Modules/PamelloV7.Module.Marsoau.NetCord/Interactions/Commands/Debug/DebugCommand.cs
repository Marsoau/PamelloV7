using System.Text.Json;
using PamelloV7.Framework.Converters;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Debug;

[DiscordCommand("debug command", "Debug command")]
public partial class DebugCommand
{
    public async Task Execute(
        [Description("path", "Command path to execute")] string path
    ) {
        var result = await WithLoadingAsync(Command(path));
        var json = JsonSerializer.Serialize(result, JsonEntitiesFactory.Options);
        
        await RespondAsync(path, DiscordString.CodeBlock(json, "json"));
    }
}
