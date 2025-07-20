using PamelloV7.Core.Data.Entities.Base;

namespace PamelloV7.Core.Data.Entities;

public class DatabasePlaylist : DatabaseEntity
{
    public string Name { get; }
    public IEnumerable<int> SongIds { get; }
    public int OwnerId { get; }
    public bool IsProtected { get; }
    public DateTime AddedAt { get; }
}
