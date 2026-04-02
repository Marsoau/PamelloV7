using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;

public abstract class DiscordButton : DiscordInteraction<ButtonInteraction>
{
    protected override Task RespondLoadingInternal() {
        return Task.CompletedTask;
    }

    protected override Task ReleaseInteractionInternal() {
        return Interaction.SendResponseAsync(InteractionCallback.ModifyMessage(_ => { }));
    }
}
