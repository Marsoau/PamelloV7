using PamelloV7.Framework.Data.Entities.Base;

namespace PamelloV7.Framework.Data.Entities;

public class DatabasePlaylist : DatabaseEntity
{
    public required string Name { get; set; }
    public required List<int> SongIds { get; set; }
    public required int OwnerId { get; set; }
    public required bool IsProtected { get; set; }
    public required DateTime AddedAt { get; set; }
}
