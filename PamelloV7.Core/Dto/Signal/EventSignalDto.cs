namespace PamelloV7.Core.Dto.Signal;

public record EventSignalDto(
    EventTypeInfo Type,
    List<EventTypeInfo> NestedTypes,
    object Data
);
