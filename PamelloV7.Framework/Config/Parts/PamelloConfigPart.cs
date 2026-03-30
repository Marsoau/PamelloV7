using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config.Attributes;
using PamelloV7.Framework.Extensions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Framework.Config.Parts;

public class PamelloConfigPart
{
    public Type? _nodeType = null!;
    public Type? _staticType = null!;
    
    public string Name { get; }
    public JsonNode Json { get; }
    public Type NodeType => _nodeType ?? throw new Exception("Node type not initialized");
    public Type StaticType => _staticType ?? throw new Exception("Static type not initialized");
    public object? Node { get; private set; } = null;
    public IPamelloModule? Module { get; private set; }
    
    public List<PamelloConfigPreInitializer> PreInitializers { get; private set; } = [];
    
    public bool IsJustCreated { get; }
    public bool IsInitialized => _nodeType != null;

    public PamelloConfigPart(string name, JsonNode json, bool isJustCreated = false) {
        Name = name;
        Json = json;
        
        IsJustCreated = isJustCreated;
    }
    
    public void Initialize(Type nodeType, Type staticType, IPamelloModule? module) {
        _nodeType = nodeType;
        _staticType = staticType;
        
        Module = module;

        PreInitializers = GetPreInitializers(nodeType).ToList();
    }

    private JsonNode? GetInnerNode(string path, bool getParent = false) {
        var currentNode = Json;

        
        var keys = path.Split('.');
        if (getParent) keys = keys.Take(keys.Length - 1).ToArray();
        
        foreach (var key in keys) {
            if (currentNode is null) return false;
            
            foreach (var (nodeKey, node) in currentNode.AsObject()) {
                if (nodeKey != key) continue;
                
                currentNode = node;
                break;
            }
        }
        
        return getParent || currentNode != Json ? currentNode : null;
    }
    
    public IEnumerable<PamelloConfigPreInitializer> GetPreInitializers(Type nodeType, string? previousPath = null) {
        var properties = nodeType.GetProperties();
        
        foreach (var property in properties) {
            var propertyPath = previousPath is null ? property.Name : $"{previousPath}.{property.Name}";
            
            if (property.PropertyType is { IsNested: true, DeclaringType: var declaringType } && declaringType == nodeType) {
                foreach (var preInitializer in GetPreInitializers(property.PropertyType, propertyPath)) {
                    yield return preInitializer;
                }
                continue;
            }

            if (property.GetCustomAttribute<RequiredMemberAttribute>() is null) continue;
            if (GetInnerNode(propertyPath) is not null) continue;
            
            yield return new PamelloConfigPreInitializer(this, propertyPath, property.PropertyType);
        }
    }

    public void Finish() {
        if (Node is not null) return;

        foreach (var preInitializer in PreInitializers) {
            if (!preInitializer.IsPreInitialized) throw new PamelloException($"Not all pre-initializers have value (\"{preInitializer}\")");
            
            var node = GetInnerNode(preInitializer.PropertyPath, true)?.AsObject();
            if (node is null) continue;
            
            node.Add(preInitializer.PropertyPath.Split('.').Last(), JsonSerializer.SerializeToNode(preInitializer.Value));
        }

        Node = Json.Deserialize(NodeType);
        
        var rootField = StaticType.GetField("Root");
        var partField = StaticType.GetField("Part");
        
        rootField?.SetValue(null, Node);
        partField?.SetValue(null, this);
    }
    
    public override string ToString() => Name;
}
