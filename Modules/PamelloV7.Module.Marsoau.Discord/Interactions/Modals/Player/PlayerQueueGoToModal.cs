using Discord;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Player;

public class PlayerQueueGoToModal : DiscordModal
{
    public static Modal Build() {
        var modalBuilder = new ModalBuilder()
            .WithTitle("Go-To Position in Queue")
            .WithCustomId($"player-queue-goto-modal")
            .AddComponents(new ModalComponentBuilder()
                .WithTextInput("Position", new TextInputBuilder()
                    .WithCustomId("position-input")
                    .WithRequired(true)
                )
                .WithSelectMenu("Return Back", GetYesNoSelect("return-back", false))
            );
        
        return modalBuilder.Build();
    }

    [ModalSubmission("player-queue-goto-modal")]
    public async Task Submit() {
        Output.Write("exe");
        var position = GetInputValue("position-input");
        var returnBack = GetYesNoValue("return-back");

        Command<PlayerQueueGoTo>().Execute(position, returnBack);
        
        await ReleaseInteractionAsync();
    }
}
