using Discord;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Player;

public class PlayerQueueSetNextModal : DiscordModal
{
    public static Modal Build() {
        var modalBuilder = new ModalBuilder()
            .WithTitle("Set Next Position in Queue")
            .WithCustomId($"player-queue-set-next-modal")
            .AddComponents(new ModalComponentBuilder()
                .WithTextInput("Position", new TextInputBuilder()
                    .WithCustomId("position-input")
                    .WithRequired(false)
                )
            );
        
        return modalBuilder.Build();
    }

    [ModalSubmission("player-queue-set-next-modal")]
    public async Task Submit() {
        var position = GetInputValue("position-input");

        Command<PlayerQueueRequestNextPosition>().Execute(position);
        
        await ReleaseInteractionAsync();
    }
}
