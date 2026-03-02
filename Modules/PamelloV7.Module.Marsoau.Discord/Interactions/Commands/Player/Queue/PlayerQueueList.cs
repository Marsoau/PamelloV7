using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Player;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player.Queue;

public partial class PlayerQueue
{
    [SlashCommand("list", "List the queue")]
    public async Task List() {
        await RespondUpdatablePageAsync(page =>
            Builder<QueueListBuilder>().Get(page, 10).Build()
        , () => [ScopeUser.SelectedPlayer, ScopeUser]);
    }
}

public partial class PlayerQueueInteractions
{
    [ComponentInteraction("player-queue-goto")]
    public async Task GoToButton() {
        await RespondWithModalAsync(PlayerQueueGoToModal.Build());
    }
    [ComponentInteraction("player-queue-set-next")]
    public async Task SetNextButton() {
        await RespondWithModalAsync(PlayerQueueSetNextModal.Build());
    }
}
