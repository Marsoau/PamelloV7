using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Model.PEQL;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.PEQL.Model;

namespace PamelloV7.Module.Marsoau.PEQL.Services;

public class EntityQueryService : IEntityQueryService
{
    private readonly IServiceProvider _services;
    
    private readonly IPamelloLogger _logger;
    
    public readonly List<EntityProviderContainer> Providers;
    public readonly List<EntityOperatorDescriptor> Operators;
    
    public EntityQueryService(IServiceProvider services) {
        _services = services;
        
        _logger = services.GetRequiredService<IPamelloLogger>();
        
        Providers = [];
        Operators = [];
    }
    
    public void LoadProviders(IServiceCollection collection, IServiceProvider services) {
        _logger.Log("Loading entity providers");
        
        foreach (var descriptor in collection) {
            if (!descriptor.ServiceType.IsAssignableTo(typeof(IEntityProvider))) continue;
            var attribute = descriptor.ServiceType.GetCustomAttribute<EntityProviderAttribute>();
            if (attribute is null) continue;
            
            var provider = services.GetRequiredService(descriptor.ServiceType);
            var container = new EntityProviderContainer(attribute.Name, descriptor.ServiceType, provider);
            
            Providers.Add(container);
            
            Console.WriteLine($"| {container.Name}: {container.Type.Name}");
        }
        
        _logger.Log($"Loaded {Providers.Count} entity providers");
    }
    
    public void LoadOperators(IServiceProvider services) {
        _logger.Log("Loading PEQL operators");
        
        var typeResolver = services.GetRequiredService<IAssemblyTypeResolver>();
        
        var operatorTypes = typeResolver.GetInheritors<EntityOperator>();
        
        foreach (var operatorType in operatorTypes) {
            var attribute = operatorType.GetCustomAttribute<EntityOperatorAttribute>();
            if (attribute is null) continue;
            
            var descriptor = new EntityOperatorDescriptor(attribute.Name, attribute.Symbol, operatorType);
            
            Operators.Add(descriptor);
            
            Console.WriteLine($"| {descriptor.Symbol} : {descriptor.Name}");
        }
        
        _logger.Log($"Loaded {Providers.Count} operators");
    }

    public IEnumerable<IPamelloEntity> Get(string query, IPamelloUser scopeUser) {
        var split = query.Split('$');
        if (split.Length != 2) throw new Exception("Invalid query");
        
        var context = split[0];
        var value = split[1];
        
        var provider = Providers.FirstOrDefault(provider => provider.Name == context);
        if (provider is null) throw new Exception($"Provider {context} not found");
        
        var results = new List<IPamelloEntity>();

        for (var i = value.Length - 1; i >= 0; i--) {
            if (value[i] != ',') continue;
            
            var firstQuery = value[..i];
            var secondQuery = value[(i + 1)..];
            
            results.AddRange(Get($"{context}${firstQuery}", scopeUser));
            results.AddRange(Get($"{context}${secondQuery}", scopeUser));
            return results;
        }

        if (int.TryParse(value, out var id)) {
            return [provider.GetById(id, scopeUser)];
        }
        
        for (var i = value.Length - 1; i >= 0; i--) {
            var descriptor = Operators.FirstOrDefault(descriptor => descriptor.Symbol == value[i]);
            if (descriptor is null) continue;
            
            var operatorQuery = value[..i];
            var operatorValue = value[(i + 1)..];

            var op = (EntityOperator)Activator.CreateInstance(descriptor.Type, _services)!;
            
            results.AddRange(op.Execute(scopeUser, $"{context}${operatorQuery}", operatorValue));
            return results;
        }
        
        results.AddRange(provider.GetFromPoint(value, scopeUser, []));
        
        return results;
    }
}
