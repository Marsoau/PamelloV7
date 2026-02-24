using PamelloV7.Framework.Attributes;

namespace PamelloV7.Framework.Entities.Other;

[SafeEntity<IPamelloSong>("Song")]
[SafeEntity<IPamelloUser>("Adder")]
public partial record PamelloQueueEntry
{
    public PamelloQueueEntry() { }
    public PamelloQueueEntry(IPamelloSong song, IPamelloUser adder) {
        Song = song;
        Adder = adder;
    }
};
