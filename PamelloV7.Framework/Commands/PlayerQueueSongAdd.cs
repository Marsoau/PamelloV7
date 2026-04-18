using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueSongAdd
{
    public IEnumerable<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs, string? position = null) {
        if (position is not null)
            return RequiredQueue.InsertSongs(position, songs, ScopeUser);
        
        return RequiredQueue.AddSongs(songs, ScopeUser);
    }
}
