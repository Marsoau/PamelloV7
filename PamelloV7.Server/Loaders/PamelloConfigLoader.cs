using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Config.Loaders;
using PamelloV7.Framework.Config.Parts;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Loaders;

public class PamelloConfigLoader : IPamelloConfigLoader
{
    public JsonObject? _json;
    public JsonObject Json => _json ?? throw new InvalidOperationException("Json not set");
    
    public List<PamelloConfigPart> Parts { get; private set; }

    private readonly JsonSerializerOptions _jsoncProperties;

    public PamelloConfigLoader() {
        Parts = [];
        
        _jsoncProperties = new JsonSerializerOptions {
            ReadCommentHandling = JsonCommentHandling.Skip
        };
    }

    public void Load() {
        var configFile = new FileInfo(Program.ConfigPath);
        if (!configFile.Exists) throw new PamelloLoadingException($"Config file by path \"{Program.ConfigPath}\" not found");

        //Output.Write($"Loading config from file \"{configFile.FullName}\"", ELogLevel.Debug);
            
        using var fs = configFile.OpenRead();
        
        _json = JsonSerializer.Deserialize<JsonObject>(fs, _jsoncProperties);

        foreach (var (partName, partJson) in Json) {
            if (partJson is null) continue;
            
            Parts.Add(new PamelloConfigPart(partName, partJson));
        }
    }

    public void InitializeFromContainer(PamelloModuleContainer container) {
        foreach (var (partName, staticAndNodeType) in container.ConfigTypes) {
            var fullName = $"{container.Module.Author}/{container.Module.Name}{
                (!string.IsNullOrWhiteSpace(partName) ? $":{partName}" : "")
            }";
            var (partStaticType, partNodeType) = staticAndNodeType;
            
            var part = Parts.FirstOrDefault(part => part.Name == fullName);
            if (part is null) {
                var json = JsonSerializer.SerializeToNode(partNodeType);
                if (json is null) throw new PamelloLoadingException($"Config part \"{fullName}\" is not serializable");

                part = new PamelloConfigPart(fullName, json);
                
                Parts.Add(part);
            }
            
            part.Initialize(partNodeType, partStaticType, container.Module);
        }
    }

    public void FinishFromContainer(PamelloModuleContainer container) {
        Parts.Where(part => part.Module == container.Module).ToList().ForEach(part => part.Finish());
    }

    /*
    public void InitType(Type type, string partName) {
        var rootProperty = type.GetField("Root");
        if (rootProperty is null) throw new PamelloLoadingException($"Root property not found in config type {type.FullName}");

        var container = Parts.FirstOrDefault(x => x.Name == partName);
        if (container is null) throw new PamelloLoadingException($"Config part \"{partName}\" not found in config file");
        if (container.Json.ValueKind == JsonValueKind.Null) throw new PamelloLoadingException($"Config part \"{container.Name}\" not found in config file");

        foreach (var property in rootProperty.FieldType.GetProperties()) {
            Output.Write($"| {property.Name}: {property.PropertyType}", ELogLevel.Debug);
        }

        rootProperty.SetValue(null, container.Json.Deserialize(rootProperty.FieldType, _jsoncProperties));
    }
    */
}
