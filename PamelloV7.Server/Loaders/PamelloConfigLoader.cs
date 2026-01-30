using System.Reflection;
using System.Text.Json;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Config;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Loaders;

public class PamelloConfigContainer
{
    public string PartName { get; }
    public JsonElement Part { get; set; }
    
    public PamelloConfigContainer(string partName, JsonElement part) {
        PartName = partName;
        Part = part;
    }
}

public class PamelloConfigLoader
{
    public List<PamelloConfigContainer> Containers { get; private set; }

    private readonly JsonSerializerOptions _jsoncProperties;

    public PamelloConfigLoader() {
        Containers = [];
        
        _jsoncProperties = new JsonSerializerOptions {
            ReadCommentHandling = JsonCommentHandling.Skip
        };
    }

    public void Load() {
        var configFile = new FileInfo(Program.ConfigPath);
        if (!configFile.Exists) throw new PamelloLoadingException($"Config file by path \"{Program.ConfigPath}\" not found");
        
        StaticLogger.Log($"Loading config from file \"{configFile.FullName}\"");
            
        var fs = configFile.OpenRead();

        var jsonParts = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(fs, _jsoncProperties);
        if (jsonParts is null) throw new PamelloLoadingException("Failed to load config.json");
            
        fs.Close();

        StaticLogger.Log($"Loaded json file parts: ({jsonParts.Count} parts)");
        foreach (var (partName, partJson) in jsonParts) {
            Containers.Add(new PamelloConfigContainer(partName, partJson));
            Console.WriteLine($"{partName}: {partJson}");
        }
    }

    public void InitType(Type type, string partName) {
        var rootProperty = type.GetProperty("Root");
        if (rootProperty is null) throw new PamelloLoadingException($"Root property not found in config type {type.FullName}");
            
        if (!rootProperty.PropertyType.IsAssignableTo(typeof(IConfigNode))) throw new PamelloLoadingException($"Root property should implement IConfigNode");
            
        var container = Containers.FirstOrDefault(x => x.PartName == partName);
        if (container is null) throw new PamelloLoadingException($"Config part \"{partName}\" not found in config file");
        if (container.Part.ValueKind == JsonValueKind.Null) throw new PamelloLoadingException($"Config part \"{container.PartName}\" not found in config file");
            
        rootProperty.SetValue(null, container.Part.Deserialize(rootProperty.PropertyType, _jsoncProperties));
    }
}
