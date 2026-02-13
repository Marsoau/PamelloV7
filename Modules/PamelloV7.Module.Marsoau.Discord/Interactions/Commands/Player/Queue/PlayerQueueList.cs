using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Builders;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player.Queue;

public partial class PlayerQueue
{
    [SlashCommand("list", "List the queue")]
    public async Task List() {
        await RespondUpdatablePageAsync(page => 
            Builder<QueueListBuilder>().Get(page, 10).Build()
        , () => [ScopeUser]);
    }
}
