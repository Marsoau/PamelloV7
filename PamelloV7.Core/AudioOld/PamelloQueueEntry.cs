using PamelloV7.Core.Entities;

namespace PamelloV7.Core.AudioOld;

public record PamelloQueueEntry(
    IPamelloSong Song,
    IPamelloUser? Adder
);
