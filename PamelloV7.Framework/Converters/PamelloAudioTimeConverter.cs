using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Core.Audio;
using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Converters;

public class PamelloAudioTimeConverter : JsonConverter<AudioTime>
{
    public override AudioTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return new AudioTime(reader.GetInt32());
    }

    public override void Write(Utf8JsonWriter writer, AudioTime value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.TotalSeconds);
    }
}
