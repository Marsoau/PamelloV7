using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Converters;

public class SafeStoredEntitiesConverter<TEntityType> : JsonConverter<SafeStoredEntities<TEntityType>>
    where TEntityType : class, IDeletableEntity
{
    public override SafeStoredEntities<TEntityType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected an array of IDs for {typeof(SafeStoredEntities<TEntityType>).Name}.");

        var ids = new List<int>();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var id)) {
                ids.Add(id);
            }
            else {
                throw new JsonException("Expected an integer ID in the array.");
            }
        }

        return new SafeStoredEntities<TEntityType>(ids);
    }

    public override void Write(Utf8JsonWriter writer, SafeStoredEntities<TEntityType> value, JsonSerializerOptions options) {
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