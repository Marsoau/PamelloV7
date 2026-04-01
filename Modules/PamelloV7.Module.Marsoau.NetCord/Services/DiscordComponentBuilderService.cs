using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public class DiscordComponentBuilderService : IPamelloService
{
    private readonly IServiceProvider _services;

    public DiscordComponentBuilderService(IServiceProvider services) {
        _services = services;
    }
    
    public TBuilder Get<TBuilder>(IPamelloUser scopeUser)
        where TBuilder : DiscordComponentBuilder
        => (TBuilder)Get(typeof(TBuilder), scopeUser);
    
    public DiscordComponentBuilder Get(Type builderType, IPamelloUser scopeUser) {
        var constructor = builderType.GetConstructor([]);
        if (constructor is null) throw new PamelloException($"Builder {builderType.FullName} does not have a default constructor");
        
        var builder = (DiscordComponentBuilder)constructor.Invoke([]);
        
        builder.Initialize(_services, scopeUser);
        
        return builder;
    }
}
