using Discord;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Context;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Builders.Base;

public abstract class PamelloDiscordComponentBuilder
{
    public readonly IServiceProvider Services;
    public readonly PamelloSocketInteractionContext Context;
    
    protected IPamelloUser ScopeUser => Context.User;
    
    protected IPamelloPlayer? SelectedPlayer => ScopeUser.SelectedPlayer;
    
    public TBuilder Builder<TBuilder>()
        where TBuilder : PamelloDiscordComponentBuilder
    {
        var builders = Services.GetRequiredService<DiscordComponentBuildersService>();
        return builders.GetBuilder<TBuilder>(Context);
    }
}
