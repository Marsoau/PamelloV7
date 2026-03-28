using System.Reflection;
using PamelloV7.Framework.Config.Attributes;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Modules.Containers;

public class PamelloModuleContainer
{
    public readonly Assembly Assembly;
    public readonly IPamelloModule Module;
    public readonly Dictionary<Type, Type?> Services;
    public readonly List<string> Dependencies;
    public readonly Dictionary<string, KeyValuePair<Type, Type>> ConfigTypes;
    
    public PamelloModuleContainer(Assembly assembly, IPamelloModule module) {
        Assembly = assembly;
        Module = module;
        Services = [];
        ConfigTypes = [];
        
        var serviceTypes = assembly.GetTypes().Where(x => typeof(IPamelloService).IsAssignableFrom(x));
        foreach (var service in serviceTypes) {
            var serviceInterface = service.GetInterfaces()
                .FirstOrDefault(i => i != typeof(IPamelloService) && typeof(IPamelloService).IsAssignableFrom(i));
            
            Services.Add(service, serviceInterface);
        }
        
        var referencedAssemblies = assembly.GetReferencedAssemblies();
        Dependencies = referencedAssemblies
            .Select(x => x.Name!)
            .Where(name => name.StartsWith("PamelloV7.Module"))
            .ToList();
        
        var rootNodes = assembly.GetTypes().Where(t => t.GetCustomAttribute<ConfigRootAttribute>() is not null);
        foreach (var rootNode in rootNodes) {
            var attribute = rootNode.GetCustomAttribute<ConfigRootAttribute>()!;
            
            var staticType = assembly.GetTypes().FirstOrDefault(t => t.Name == rootNode?.Name.Replace("Node", "Config"));
            var nodeType = assembly.GetTypes().FirstOrDefault(t => t.Name == rootNode?.Name);
        
            if (staticType is null || nodeType is null) return;
        
            ConfigTypes.Add(attribute.Name?.Split(":").LastOrDefault() ?? "", new KeyValuePair<Type, Type>(staticType, nodeType));
        }
    }

    public override string ToString() {
        return $"[{Module.Author}/{Module.Name}] ({Services.Count} services)";
    }
}

