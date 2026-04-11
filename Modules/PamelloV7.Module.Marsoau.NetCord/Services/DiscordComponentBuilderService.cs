using System.Collections.Concurrent;
using NetCord;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public class DiscordComponentBuilderService : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly ConcurrentDictionary<InteractionCallSite, DiscordComponentBuilder> _builders = [];

    public DiscordComponentBuilderService(IServiceProvider services) {
        _services = services;
    }
    
    public TBuilder Get<TBuilder>(InteractionCallSite callSite, IPamelloUser scopeUser)
        where TBuilder : DiscordComponentBuilder
        => (TBuilder)Get(typeof(TBuilder), callSite, scopeUser);
    
    public DiscordComponentBuilder Get(Type builderType, InteractionCallSite callSite, IPamelloUser scopeUser) {
        var constructor = builderType.GetConstructor([]);
        if (constructor is null) throw new PamelloException($"Builder {builderType.FullName} does not have a default constructor");

        var builder = _builders.GetValueOrDefault(callSite);
        builder ??= _builders[callSite] = (DiscordComponentBuilder)constructor.Invoke([]);
        
        builder.InitializeComponentBuilder(callSite.Differentiator, _services, scopeUser);
        
        return builder;
    }
}
