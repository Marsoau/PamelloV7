using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Debug;

[DiscordCommand("debug query", "Run a PEQL query and see its results")]
public partial class DebugQuery
{
    public async Task Execute(
        [Description("query", "Full query (with a $)")] string query,
        [Description("display-names", "Dispaly entities type names")] bool displayNames = false
    ) {
        var entities = await GetAsync(query);

        await RespondPageAsync(async page => 
            Builder<BasicComponentsBuilder>().EntitiesList("Query Result", entities, page, displayNames)
        , () => [..entities]);
    }
}
