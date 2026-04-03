using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

public class TokenizedModalInteraction : TokenizedInteraction
{
    protected Func<ButtonInteraction, Task<DiscordModalBuilder>> CreateModalBuilder { get; }
    protected Func<ModalInteraction, Task<DiscordModal>> CreateModal { get; }
    
    protected Func<DiscordModalBuilder, Task<ModalProperties>> BuildModal { get; }
    protected Func<DiscordModal, Task> SubmitModal { get; }

    public TokenizedModalInteraction(
        Func<ButtonInteraction, Task<DiscordModalBuilder>> createModalBuilder,
        Func<ModalInteraction, Task<DiscordModal>> createModal,
        Func<DiscordModalBuilder, Task<ModalProperties>> buildModal,
        Func<DiscordModal, Task> submitModal
    ) {
        CreateModalBuilder = createModalBuilder;
        CreateModal = createModal;
        BuildModal = buildModal;
        SubmitModal = submitModal;
        
        Action = InteractionAction;
    }

    private Task InteractionAction(Interaction interaction) {
        if (interaction is ButtonInteraction buttonInteraction) return OnButtonSubmit(buttonInteraction);
        if (interaction is ModalInteraction modalInteraction) return OnModalSubmit(modalInteraction);
        
        Output.Write("Modal interaction was not a button");
        return Task.CompletedTask;
    }

    public async Task OnButtonSubmit(ButtonInteraction interaction) {
        var builder = await CreateModalBuilder(interaction);
        builder.ModalId = CustomId;
        var modalProperties = await BuildModal(builder);

        await interaction.SendResponseAsync(InteractionCallback.Modal(modalProperties));
    }

    public async Task OnModalSubmit(ModalInteraction interaction) {
        var modal = await CreateModal(interaction);
        
        await SubmitModal(modal);
    }
}
