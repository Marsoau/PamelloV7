using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Converters;

public class SafeStoredEntitiesConverter<TEntityType> : JsonConverter<SafeList<TEntityType>>
    where TEntityType : class, IDeletableEntity
{
    public override SafeList<TEntityType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected an array of IDs for {typeof(SafeList<TEntityType>).Name}.");

        var ids = new List<int>();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var id)) {
                ids.Add(id);
            }
            else {
                throw new JsonException("Expected an integer ID in the array.");
            }
        }

        return new SafeList<TEntityType>(ids);
    }

    public override void Write(Utf8JsonWriter writer, SafeList<TEntityType> value, JsonSerializerOptions options) {
        if (value is null) {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var id in value.InternalIds) {
            writer.WriteNumberValue(id);
        }
        writer.WriteEndArray();
    }
}