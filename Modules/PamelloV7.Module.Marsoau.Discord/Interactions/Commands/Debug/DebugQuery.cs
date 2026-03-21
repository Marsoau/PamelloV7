using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Debug;

[DebugGroup]
public class DebugQueryCommand : DiscordCommand
{
    [SlashCommand("query", "Display queried entities", runMode: RunMode.Async)]
    public async Task Query(
        [Summary("query", "Query to execute")] string query,
        [Summary("display-names", "Whether to display entity type names or not")] bool displayNames = false
    ) {
        var entities = await GetAsync(query);
        
        await RespondUpdatablePageAsync(page =>
            Builder<BasicComponentsBuilder>().EntitiesList("Query Result", entities, page, displayNames, $"Nothing found by query {DiscordString.Code(query)}").Build()
        , () => [.. entities]);
    }
}
