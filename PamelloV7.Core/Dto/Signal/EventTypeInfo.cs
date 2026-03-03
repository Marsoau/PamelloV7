namespace PamelloV7.Core.Dto.Signal;

public record EventTypeInfo(
    string Name,
    string Category,
    string EntityTypeName,
    string EntityPropertyName,
    string UpdatePropertyName
);
