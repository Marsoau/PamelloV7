using System.Reflection;
using System.Text;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.RestorePacks.Base;
using PamelloV7.Wrapper.Events.Base;

namespace PamelloV7.Tools.Generators;

public static class EventsDtoGenerator
{
    public static void Generate(DirectoryInfo targetDirectory) {
        var assembly = typeof(IPamelloEvent).Assembly;
        var events = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IPamelloEvent))).ToList();

        foreach (var eventType in events) {
            File.WriteAllText(Path.Combine(targetDirectory.FullName, $"{eventType.Name}.g.cs"), GetFileText(eventType));
        }
    }

    public static string GetFileText(Type eventType) {
        var sb = new StringBuilder();

        foreach (var property in eventType.GetProperties()) {
            if (property.PropertyType.IsAssignableTo(typeof(IRevertPack))) continue;
            if (property.DeclaringType != eventType) continue;
            
            var context = new NullabilityInfoContext();
            var isNullable = context.Create(property).ReadState == NullabilityState.Nullable;
            
            sb.AppendLine($"    public {CommandExtensionsGenerator.GetReturnTypeInfo(property.PropertyType, false, isNullable)} {property.Name} {{ get; set; }}");
        }
        
        var baseTypeName = "IRemoteEvent";
        if (eventType.BaseType is not null && eventType.BaseType.IsAssignableTo(typeof(IPamelloEvent))) {
            baseTypeName = eventType.BaseType.Name;
        }
        
        var content = 
            $$"""
              //auto generated
              
              using PamelloV7.Wrapper.Events.Base;
              using PamelloV7.Wrapper.Events.Attributes;
              
              namespace PamelloV7.Wrapper.Events.Dto;
              
              [EventFullName("{{eventType.FullName}}")]
              public class {{eventType.Name}} : {{baseTypeName}}
              {
              {{sb}}
              }
              """;
        
        return content;
    }
}
