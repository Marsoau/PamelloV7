using NetCord;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

public abstract class DiscordModal : DiscordInteraction<ModalInteraction>
{
    protected override Task RespondLoadingInternal() {
        throw new NotImplementedException();
    }
    protected override Task ReleaseInteractionInternal() {
        throw new NotImplementedException();
    }
}
