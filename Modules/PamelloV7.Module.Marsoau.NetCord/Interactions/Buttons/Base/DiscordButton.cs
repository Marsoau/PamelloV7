using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;

public abstract class DiscordButton : DiscordInteraction<ButtonInteraction>
{
    public override string GetCallSiteInteractionDifferentiator() {
        throw new PamelloException("Cannot create Differentiator in DiscordButton");
    }
}
