using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Actions;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Base;

public abstract class DiscordInteraction<TInteraction> : DiscordInteraction
    where TInteraction : Interaction
{
    public TInteraction Interaction => _interaction as TInteraction
       ?? throw new InvalidOperationException($"Interaction is not of type {typeof(TInteraction).FullName}");
}

public abstract class DiscordInteraction : DiscordBasicActions
{
    protected Interaction _interaction = null!;

    public virtual void InitializeInteraction(
        Interaction interaction,
        IServiceProvider services,
        IPamelloUser scopeUser
    ) {
        InitializeActions(services, scopeUser);
        
        _interaction = interaction;
    }
}
