using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Audio;

public record PamelloQueueEntry(
    IPamelloSong Song,
    IPamelloUser? Adder
);
