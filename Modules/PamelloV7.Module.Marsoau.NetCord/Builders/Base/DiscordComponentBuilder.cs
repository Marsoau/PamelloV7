using PamelloV7.Framework.Entities;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Base;

public abstract class DiscordComponentBuilder
{
    protected IServiceProvider Services = null!;
    
    protected IPamelloUser ScopeUser = null!;
    
    public void Initialize(IServiceProvider services, IPamelloUser scopeUser) {
        Services = services;
        ScopeUser = scopeUser;
    }
}
