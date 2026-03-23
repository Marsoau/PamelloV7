using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Extensions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.PEQL;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.PEQL.Model;

namespace PamelloV7.Module.Marsoau.PEQL.Services;

public class EntityQueryService : IEntityQueryService
{
    private readonly IServiceProvider _services;
    
    public readonly List<EntityProviderContainer> Providers;
    public readonly List<EntityOperatorDescriptor> Operators;
    
    public EntityQueryService(IServiceProvider services) {
        _services = services;
        
        Providers = [];
        Operators = [];
    }

    public void Startup(IServiceProvider services) {
        var collection = services.GetRequiredService<IServiceCollection>();
        
        LoadProviders(collection, services);
        LoadOperators(services);
    }

    public void LoadProviders(IServiceCollection collection, IServiceProvider services) {
        Output.Write("Loading entity providers");
        
        foreach (var descriptor in collection) {
            if (!descriptor.ServiceType.IsAssignableTo(typeof(IEntityProvider))) continue;
            var attribute = descriptor.ServiceType.GetCustomAttribute<EntityProviderAttribute>();
            if (attribute is null) continue;
            
            var provider = services.GetRequiredService(descriptor.ServiceType);
            var container = new EntityProviderContainer(attribute.Name, descriptor.ServiceType, provider, _services);
            
            Providers.Add(container);
            
            Output.Write($"| {container.Name}: {container.Type.Name}");
        }
        
        Output.Write($"Loaded {Providers.Count} entity providers");
    }
    
    public void LoadOperators(IServiceProvider services) {
        Output.Write("Loading PEQL operators");
        
        var typeResolver = services.GetRequiredService<IAssemblyTypeResolver>();
        
        var operatorTypes = typeResolver.GetInheritorsOf<EntityOperator>();
        
        foreach (var operatorType in operatorTypes) {
            var attribute = operatorType.GetCustomAttribute<EntityOperatorAttribute>();
            if (attribute is null) continue;
            
            var descriptor = new EntityOperatorDescriptor(attribute.Name, attribute.Symbol, operatorType);
            
            Operators.Add(descriptor);
            
            Output.Write($"| {descriptor.Symbol} : {descriptor.Name}");
        }
        
        Output.Write($"Loaded {Operators.Count} operators");
    }

    public async Task<List<IPamelloEntity>> GetAsync(string query, IPamelloUser scopeUser)
        => (await InternalGetAsync(query, scopeUser))
            .Where(e => e is not null)
            .ToList();

    public IPamelloEntity? GetById(Type entityType, int id) {
        var attribute = entityType.GetCustomAttribute<PamelloEntityAttribute>();
        if (attribute is null) return null;
        
        var entity = Providers.FirstOrDefault(provider => provider.Name == attribute.ProviderName)?.GetById(id, null);
        
        if (entity?.IsDeleted ?? false) return null;
        
        return entity;
    }

    private async Task<List<IPamelloEntity>> InternalGetAsync(string query, IPamelloUser scopeUser) {
        if (scopeUser is null) throw new PamelloException("User is required to execute PEQL queries");
        
        var splitAt = -1;
        var context = "";
        var value = "";
        
        var results = new List<IPamelloEntity>();
        
        var queryParts = query.SplitArgs(',');
        if (queryParts.Length != 1) {
            foreach (var part in queryParts) {
                splitAt = part.IndexOf('$');

                if (splitAt == -1) {
                    if (context.Length == 0) throw new PamelloException("Wrong query context format");

                    value = part;
                }
                else {
                    context = part[..splitAt];
                    value = part[(splitAt + 1)..];
                }
                
                results.AddRange(await GetAsync($"{context}${value}", scopeUser));
            }
            
            return results;
        }
        
        splitAt = query.IndexOf('$');
        if (splitAt == -1) throw new PamelloException("Query does not contain provider context");

        context = query[..splitAt];
        value = query[(splitAt + 1)..];
        
        var provider = Providers.FirstOrDefault(provider => provider.Name == context);
        if (provider is null) throw new PamelloException($"Provider \"{context}\" not found");

        if (int.TryParse(value, out var id)) {
            return [provider.GetById(id, scopeUser)];
        }
        
        var quotesDepth = 0;

        for (var i = value.Length - 1; i >= 0; i--) {
            switch (value[i]) {
                case ')': quotesDepth++; continue;
                case '(': quotesDepth--; continue;
            }
            if (quotesDepth > 0) continue;
            
            var descriptor = Operators.FirstOrDefault(descriptor => descriptor.Symbol == value[i]);
            if (descriptor is null) continue;
            
            var operatorQuery = value[..i];
            var operatorValue = value[(i + 1)..];

            var op = (EntityOperator)Activator.CreateInstance(descriptor.Type, _services)!;

            try {
                results.AddRange(await op.ExecuteAsync(scopeUser, $"{context}${operatorQuery}", operatorValue));
            }
            catch (PEQLOperatorException) {
                continue;
            }
            
            return results;
        }

        var args = "";
        
        var qIndex = value.IndexOf('(');
        if (qIndex != -1) {
            if (value.Last() != ')') throw new PamelloException("Missing closing parenthesis");
            
            args = value[(qIndex + 1)..^1];
            value = value[..qIndex];
        }
        
        results.AddRange(await provider.GetFromPointAsync(value, args, scopeUser));
        
        return results;
    }
}
