using System.Reflection;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Module.Marsoau.PEQL.Model;

public class EntityProviderContainer
{
    public string Name { get; }
    public Type Type { get; }
    public readonly object Provider;
    
    public readonly List<string> ValuePointNames;
    
    public EntityProviderContainer(string name, Type type, object provider)
    {
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

    public IEnumerable<IPamelloEntity> GetFromPoint(string pointName, IPamelloUser scopeUser, object?[] args) {
        var method = Type.GetMethods().FirstOrDefault(method => method.GetCustomAttribute<ValuePointAttribute>()?.Name == pointName);
        if (method is null) throw new Exception($"Value point {pointName} not found");
        
        return (IEnumerable<IPamelloEntity>)method.Invoke(Provider, new object[] {scopeUser}.Concat(args).ToArray())!;
    }
}
