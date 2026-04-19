using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueSongAdd
{
    public IEnumerable<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs, string position = "last") {
        return RequiredQueue.InsertSongs(position, songs, ScopeUser);
    }
}
