using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Converters;

public class SafeStoredEntityConverter<TEntityType> : JsonConverter<Safe<TEntityType>>
    where TEntityType : class, IDeletableEntity
{
    public override Safe<TEntityType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return reader.TokenType switch {
            JsonTokenType.Null => new Safe<TEntityType>(),
            JsonTokenType.Number when reader.TryGetInt32(out var id) => new Safe<TEntityType>(id),
            _ => throw new JsonException($"Expected an integer ID for {typeof(Safe<TEntityType>).Name}.")
        };
    }

    public override void Write(Utf8JsonWriter writer, Safe<TEntityType> value, JsonSerializerOptions options) {
        if (value is null) {
            writer.WriteNullValue();
            return;
        }

        writer.WriteNumberValue(value.Id);
    }
}
