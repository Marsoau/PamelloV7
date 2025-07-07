using PamelloV7.Core.Model.Entities;

namespace PamelloV7.Core.Model.Audio;

public record PamelloQueueEntry(
    IPamelloSong Song,
    IPamelloUser? Adder
);
