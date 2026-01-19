using System.Reflection;
using System.Text.Json;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Config;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Loaders;

public class PamelloConfigContainer
{
    public string Name { get; }
    public Type Type { get; }
    
    public PamelloConfigContainer(string name, Type type) {
        Name = name;
        Type = type;
    }
}

public class PamelloConfigLoader
{
    public List<PamelloConfigContainer> Containers { get; private set; }

    public PamelloConfigLoader() {
        Containers = [];
    }

    public void Load() {
        var configFile = new FileInfo(Program.ConfigPath);
        if (!configFile.Exists) throw new PamelloLoadingException($"Config file by path \"{Program.ConfigPath}\" not found");
        
        StaticLogger.Log($"Loading config from file \"{configFile.FullName}\"");
            
        var fs = configFile.OpenRead();
        
        var jsonParts = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(fs);
        if (jsonParts is null) throw new PamelloLoadingException($"Failed to load config.json");
            
        fs.Close();

        StaticLogger.Log($"Loaded json file parts: ({jsonParts.Count} parts)");
        foreach (var (partName, partJson) in jsonParts) {
            Console.WriteLine($"{partName}: {partJson}");
        }
        
        var all = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
        
        Containers = all.Select(x => {
            var attribute = x.GetCustomAttribute<StaticConfigPartAttribute>();
            if (attribute is null) return null;
            
            return new PamelloConfigContainer(attribute.Name, x);
        }).OfType<PamelloConfigContainer>().ToList();

        StaticLogger.Log($"Loading assembly static parts: ({Containers.Count} parts)");

        foreach (var container in Containers) {
            var rootProperty = container.Type.GetProperty("Root");
            if (rootProperty is null) throw new PamelloLoadingException($"Root property not found in config part: {container.Name}");
            
            if (!rootProperty.PropertyType.IsAssignableTo(typeof(IConfigNode))) throw new PamelloLoadingException($"Root property should implement IConfigNode: {container.Name}");
            
            var partJson = jsonParts.GetValueOrDefault(container.Name);
            if (partJson.ValueKind == JsonValueKind.Null) throw new PamelloLoadingException($"Config part \"{container.Name}\" not found in config file");
            
            rootProperty.SetValue(null, JsonSerializer.Deserialize(partJson, rootProperty.PropertyType));
            
            Console.WriteLine($"| {container.Name}: {rootProperty.PropertyType.FullName}");
        }
        
        StaticLogger.Log($"Loaded {Containers.Count} parts");
    }
}
