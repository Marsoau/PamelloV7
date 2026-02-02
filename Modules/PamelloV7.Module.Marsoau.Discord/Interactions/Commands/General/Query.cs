using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.General;

[Map]
public class Query : DiscordCommand
{
    [SlashCommand("query", "Display queried entities", runMode: RunMode.Async)]
    public async Task ExecuteAsync(
        [Summary("query", "Query to execute")] string query
    ) {
        var entities = await GetAsync(query);
        
        await RespondUpdatablePageAsync(page =>
            PamelloComponentBuilders.EntitiesList("Query Result", entities, page).Build()
        , () => [.. entities]);
    }
}
