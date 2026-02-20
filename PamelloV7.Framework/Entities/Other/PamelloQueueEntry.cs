namespace PamelloV7.Framework.Entities.Other;

public record PamelloQueueEntry(
    IPamelloSong Song,
    IPamelloUser? Adder
);
