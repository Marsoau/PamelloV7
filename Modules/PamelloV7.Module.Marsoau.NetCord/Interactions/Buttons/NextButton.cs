using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Messages;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons;

[DiscordButton("Next")]
public partial class NextButton
{
    public async Task Execute() {
        if (Interaction.Message.InteractionMetadata is null) return;
        
        var messages = Services.GetRequiredService<UpdatableMessageService>();

        var message = messages.Get(Interaction.Message.InteractionMetadata.Id);
        if (message is not UpdatablePageMessage pageMessage) return;
        
        await pageMessage.SetPage(pageMessage.Page + 1);
    }
}
