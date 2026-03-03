namespace PamelloV7.Core.Dto.Signal;

public record EventSignalDto(
    List<EventTypeInfo> Types,
    object Data
);
