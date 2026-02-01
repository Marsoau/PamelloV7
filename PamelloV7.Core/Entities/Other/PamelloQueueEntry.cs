namespace PamelloV7.Core.Entities.Other;

public record PamelloQueueEntry(
    IPamelloSong Song,
    IPamelloUser? Adder
);
