using Discord.Interactions;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.General;

[Map]
public class Query : DiscordCommand
{
    [SlashCommand("query", "Display queried entities")]
    public async Task ExecuteAsync(
        [Summary("query", "Query to execute")] string query
    ) {
        var entities = _peql.Get(query, Context.User);
        
        var pageSize = 10;

        var totalPages = entities.Count / pageSize + (entities.Count % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;

        await RespondUpdatablePageAsync((message, page) => {
            message.Components = PamelloComponentBuilders.PageButtons(PamelloComponentBuilders.Info("Query Result", 
                entities.Count == 0 ? "Nema rezultata" :
                    string.Join("\n", entities.Skip(page * pageSize).Take(pageSize).Select(entity => $"{DiscordString.Code(entity.GetType().Name)} {entity.ToDiscordString()}"))
            ), page != 0, page < totalPages - 1).Build();
        }, () => [.. entities]);
    }
}
