using Microsoft.Extensions.DependencyInjection;
using NetCord;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

public abstract class DiscordCommand
{
    public IServiceProvider Services { get; private set; } = null!;
    
    public SlashCommandInteraction Interaction { get; private set; } = null!;
    public IPamelloUser ScopeUser { get; private set; } = null!;
    
    public IEntityQueryService PEQL { get; private set; } = null!;
    
    public void Initialize(IServiceProvider services, SlashCommandInteraction interaction, IPamelloUser scopeUser) {
        Services = services;
        
        Interaction = interaction;
        ScopeUser = scopeUser;
        
        PEQL = services.GetRequiredService<IEntityQueryService>();
    }
}
