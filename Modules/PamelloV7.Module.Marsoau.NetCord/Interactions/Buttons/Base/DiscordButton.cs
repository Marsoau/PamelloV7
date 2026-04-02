using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;

public abstract class DiscordButton : DiscordInteraction<ButtonInteraction>
{
    public override Task RespondLoading() {
        return Task.CompletedTask;
    }

    public override Task ReleaseInteraction() {
        return Interaction.SendResponseAsync(InteractionCallback.ModifyMessage(_ => { }));
    }
}
