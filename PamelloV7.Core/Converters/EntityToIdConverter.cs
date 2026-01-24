using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.Converters;

public class EntityToIdConverter<TEntity> : JsonConverter<TEntity> where TEntity : IPamelloEntity
{
    private readonly IServiceProvider? _services;
    private readonly IPamelloUser? _scopeUser;
    
    public EntityToIdConverter() { }

    public EntityToIdConverter(IServiceProvider services, IPamelloUser scopeUser) {
        _services = services;
        _scopeUser = scopeUser;
    }
    
    public override TEntity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return default;
    }

    public override void Write(Utf8JsonWriter writer, TEntity value, JsonSerializerOptions options) {
        writer.WriteNumberValue(value.Id);
    }
}
