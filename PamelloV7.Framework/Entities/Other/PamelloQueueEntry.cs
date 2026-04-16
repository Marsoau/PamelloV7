using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Attributes;

namespace PamelloV7.Framework.Entities.Other;

[Safe<IPamelloSong>("Song")]
[Safe<IPamelloUser>("Adder")]
public partial record PamelloQueueEntry
{
    public PamelloQueueEntry() { }
    public PamelloQueueEntry(IPamelloSong song, IPamelloUser? adder) {
        Song = song;
        Adder = adder;
    }
};
