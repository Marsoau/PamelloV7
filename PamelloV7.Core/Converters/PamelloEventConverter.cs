using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Converters;

public class PamelloEventConverter : JsonConverter<IPamelloEvent>
{
    public override IPamelloEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization of events not possible directly on asp");
    }

    public override void Write(Utf8JsonWriter writer, IPamelloEvent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
