using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Module.Marsoau.Test;

[SafeStored<IPamelloSong>("Song")]
public partial class TestContainer
{
    public TestContainer(IPamelloSong song) {
        Song = song;
    }
}
