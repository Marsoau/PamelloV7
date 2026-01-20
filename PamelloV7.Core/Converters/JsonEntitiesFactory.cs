using System.Text.Json;
using System.Text.Json.Serialization;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Converters;

public class JsonEntitiesFactory : JsonConverterFactory
{
    private readonly IServiceProvider? _services;
    private readonly IPamelloUser? _scopeUser;
    
    public JsonEntitiesFactory() { }

    public JsonEntitiesFactory(IServiceProvider services, IPamelloUser scopeUser) {
        _services = services;
        _scopeUser = scopeUser;
    }
    
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions {Converters = {new JsonEntitiesFactory()}};
    public static JsonSerializerOptions GetOptions(IServiceProvider services, IPamelloUser scopeUser)
        => new JsonSerializerOptions {Converters = {new JsonEntitiesFactory(services, scopeUser)}};
    
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsAssignableTo(typeof(IPamelloEntity));

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
        var converterType = typeof(EntityToIdConverter<>).MakeGenericType(typeToConvert);
        
        if (_services is not null && _scopeUser is not null)
            return Activator.CreateInstance(converterType, _services, _scopeUser) as JsonConverter;
        
        return Activator.CreateInstance(converterType) as JsonConverter;
    }
}
