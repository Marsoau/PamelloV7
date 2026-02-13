using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.Base;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Context;

namespace PamelloV7.Module.Marsoau.Discord.Services;

public class DiscordComponentBuildersService : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly IAssemblyTypeResolver _typeResolver;
    
    public DiscordComponentBuildersService(IServiceProvider services) {
        _services = services;
        
        _typeResolver = services.GetRequiredService<IAssemblyTypeResolver>();
    }
    
    public TBuilderType GetBuilder<TBuilderType>(PamelloSocketInteractionContext context)
        where TBuilderType : PamelloDiscordComponentBuilder
    {
        var servicesField = typeof(TBuilderType).GetField("Services");
        var contextField = typeof(TBuilderType).GetField("Context");
        if (contextField is null || servicesField is null) throw new PamelloException($"Builder {typeof(TBuilderType).FullName} should have both ScopeUser and Services fields");
        
        var builder = (TBuilderType)Activator.CreateInstance(typeof(TBuilderType))!;
        Debug.Assert(builder is not null, "Builder cannot be null here");
        
        servicesField.SetValue(builder, _services);
        contextField.SetValue(builder, context);
        
        return builder;
    }
}
