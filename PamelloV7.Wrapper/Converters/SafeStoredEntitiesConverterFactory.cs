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
    
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(SafeStoredEntities<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
        var entityType = typeToConvert.GetGenericArguments().First();
        var converterType = typeof(SafeStoredEntitiesConverter<>).MakeGenericType(entityType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
