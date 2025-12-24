using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Extensions;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Module.Marsoau.PEQL.Model;

public class EntityProviderContainer
{
    private readonly IServiceProvider _services;
    
    private readonly IEntityQueryService _peql;
    
    public string Name { get; }
    public Type Type { get; }
    public readonly object Provider;
    
    public readonly List<string> ValuePointNames;
    
    public EntityProviderContainer(string name, Type type, object provider, IServiceProvider services)
    {
        _services = services;
        
        _peql = services.GetRequiredService<IEntityQueryService>();
        
        Name = name;
        Type = type;
        Provider = provider;
        
        ValuePointNames = [];
    }

    public IPamelloEntity GetById(int id, IPamelloUser scopeUser) {
        var method = Type.GetMethods().FirstOrDefault(method => method.GetCustomAttribute<IdPointAttribute>() is not null);
        if (method is null) throw new Exception($"Provider {Name} does not support id points");
        
        return (IPamelloEntity)method.Invoke(Provider, [scopeUser, id])!;
    }

    public IEnumerable<IPamelloEntity> GetFromPoint(string pointName, string stringArgs, IPamelloUser scopeUser) {
        var method = Type.GetMethods().FirstOrDefault(method => method.GetCustomAttribute<ValuePointAttribute>()?.Is(pointName) ?? false);
        if (method is null) return [];

        var argumentsInfos = method.GetParameters();
        var stringArgsValues = stringArgs.SplitArgs(',');
        var arguments = new object?[argumentsInfos.Length - 1];

        for (var i = 0; i < argumentsInfos.Length - 1; i++) {
            var strArg = stringArgsValues.ElementAtOrDefault(i);
            var type = argumentsInfos[i + 1].ParameterType;

            if (strArg is null) {
                arguments[i] = null;
                continue;
            }

            if (type == typeof(IPamelloUser)) {
                arguments[i] = _peql.GetSingle<IPamelloUser>(strArg, scopeUser);
            }
            else if (type == typeof(IPamelloSong)) {
                arguments[i] = _peql.GetSingle<IPamelloSong>(strArg, scopeUser);
            }
            else if (type == typeof(IPamelloEpisode)) {
                arguments[i] = _peql.GetSingle<IPamelloEpisode>(strArg, scopeUser);
            }
            else if (type == typeof(IPamelloPlaylist)) {
                arguments[i] = _peql.GetSingle<IPamelloPlaylist>(strArg, scopeUser);
            }
            else {
                arguments[i] = TypeDescriptor.GetConverter(argumentsInfos[i + 1].ParameterType).ConvertFromString(strArg);
            }
        }
        
        return (IEnumerable<IPamelloEntity>)method.Invoke(Provider, new object[] {scopeUser}.Concat(arguments).ToArray())!;
    }
}
