using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Extensions;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Module.Marsoau.PEQL.Model;

public class EntityProviderContainer
{
    private readonly IServiceProvider _services;
    
    private readonly IEntityQueryService _peql;
    
    public string Name { get; }
    public Type Type { get; }
    public readonly object Provider;
    
    public readonly MethodInfo[] Methods;
    
    public EntityProviderContainer(string name, Type type, object provider, IServiceProvider services)
    {
        _services = services;
        
        _peql = services.GetRequiredService<IEntityQueryService>();
        
        Name = name;
        Type = type;
        Provider = provider;
        
        Methods = Type.GetMethods();
    }

    public IPamelloEntity GetById(int id, IPamelloUser scopeUser) {
        var method = Methods.FirstOrDefault(method => method.GetCustomAttribute<IdPointAttribute>() is not null);
        if (method is null) throw new Exception($"Provider {Name} does not support id point");
        
        return (IPamelloEntity)method.Invoke(Provider, [scopeUser, id])!;
    }
    
    public async Task<IPamelloEntity?> GetByNameAsync(string name, IPamelloUser scopeUser) {
        var method = Methods.FirstOrDefault(method => method.GetCustomAttribute<NamePointAttribute>() is not null);
        if (method is null) throw new Exception($"Provider {Name} does not have a name point");
        
        var result = method.Invoke(Provider, [scopeUser, name]);
        return result switch {
            null => null,
            Task task => await (dynamic)task as IPamelloEntity,
            _ => result as IPamelloEntity
        };
    }

    public async Task<IEnumerable<IPamelloEntity>> GetFromPointAsync(string pointName, string stringArgs, IPamelloUser scopeUser) {
        var method = Methods.FirstOrDefault(method => method.GetCustomAttribute<ValuePointAttribute>()?.Is(pointName) ?? false);
        if (method is null) {
            IPamelloEntity? nameResult;
            if (stringArgs.Length == 0) nameResult = await GetByNameAsync(pointName, scopeUser);
            else nameResult = await GetByNameAsync($"{pointName}({stringArgs})", scopeUser);
            
            if (nameResult is null) return [];
            return [nameResult];
        }

        var argumentsInfos = method.GetParameters();
        var stringArgsValues = stringArgs.Length > 0 ? stringArgs.SplitArgs(',') : [];
        var arguments = new object?[argumentsInfos.Length - 1];

        for (var i = 0; i < argumentsInfos.Length - 1; i++) {
            var strArg = stringArgsValues.ElementAtOrDefault(i);
            var type = argumentsInfos[i + 1].ParameterType;

            if (strArg is null) {
                arguments[i] = null;
                continue;
            }

            if (type == typeof(IPamelloUser)) {
                arguments[i] = await _peql.GetSingleAsync<IPamelloUser>(strArg, scopeUser);
            }
            else if (type == typeof(IPamelloSong)) {
                arguments[i] = await _peql.GetSingleAsync<IPamelloSong>(strArg, scopeUser);
            }
            else if (type == typeof(IPamelloEpisode)) {
                arguments[i] = await _peql.GetSingleAsync<IPamelloEpisode>(strArg, scopeUser);
            }
            else if (type == typeof(IPamelloPlaylist)) {
                arguments[i] = await _peql.GetSingleAsync<IPamelloPlaylist>(strArg, scopeUser);
            }
            else {
                arguments[i] = TypeDescriptor.GetConverter(argumentsInfos[i + 1].ParameterType).ConvertFromString(strArg);
            }
        }
        
        var result = method.Invoke(Provider, new object[] {scopeUser}.Concat(arguments).ToArray());
        return result switch {
            null => [],
            Task task => (IEnumerable<IPamelloEntity>) await (dynamic)task,
            _ => (IEnumerable<IPamelloEntity>)result 
        };
    }
}
