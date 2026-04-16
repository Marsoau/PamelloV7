using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Converters;

public class SafeStoredEntitiesConverterFactory : JsonConverterFactory
{
    public static readonly JsonSerializerOptions Options = new() {
        Converters = {
            new SafeStoredEntitiesConverterFactory()
        },
        UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
    };
    
    public override bool CanConvert(Type typeToConvert) {
        if (!typeToConvert.IsGenericType) return false;

        var genericType = typeToConvert.GetGenericTypeDefinition();
        
        // Return true if it's EITHER the collection OR the single entity container
        return genericType == typeof(SafeList<>) || 
               genericType == typeof(Safe<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
        var genericType = typeToConvert.GetGenericTypeDefinition();
        var entityType = typeToConvert.GetGenericArguments().First();
        
        Type converterType;

        // Route to the appropriate converter
        if (genericType == typeof(SafeList<>)) {
            converterType = typeof(SafeStoredEntitiesConverter<>).MakeGenericType(entityType);
        }
        else {
            converterType = typeof(SafeStoredEntityConverter<>).MakeGenericType(entityType);
        }
        
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
