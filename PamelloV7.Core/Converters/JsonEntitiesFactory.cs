using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Converters;

public class JsonEntitiesFactory : JsonConverterFactory
{
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions {Converters = {new JsonEntitiesFactory()}};
    
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsAssignableTo(typeof(IPamelloEntity));

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
        var converterType = typeof(EntityToIdConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as JsonConverter;
    }
}
