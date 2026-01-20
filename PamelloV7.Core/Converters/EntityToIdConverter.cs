using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Converters;

public class EntityToIdConverter<TEntity> : JsonConverter<TEntity> where TEntity : IPamelloEntity
{
    public override TEntity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return default;
    }

    public override void Write(Utf8JsonWriter writer, TEntity value, JsonSerializerOptions options) {
        writer.WriteNumberValue(value.Id);
    }
}
