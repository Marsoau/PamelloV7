using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
using PamelloV7.Module.Marsoau.NetCord.Messages;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;

public abstract class DiscordButton : DiscordInteraction<ButtonInteraction>
{
    public UpdatableMessage Message { get; private set; } = null!;

    public virtual void InitializeButton(UpdatableMessage message, ButtonInteraction interaction, IServiceProvider services, IPamelloUser scopeUser) {
        InitializeInteraction(interaction, services, scopeUser);
        
        Message = message;
    }
    
    public override Differentiator GetCallSiteInteractionDifferentiator() {
        throw new PamelloException("Cannot create Differentiator in DiscordButton");
    }
}
