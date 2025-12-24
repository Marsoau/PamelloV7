using PamelloV7.Core.Data.Entities.Base;

namespace PamelloV7.Core.Data.Entities;

public class DatabasePlaylist : DatabaseEntity
{
    public string Name { get; set; }
    public List<int> SongIds { get; set; }
    public int OwnerId { get; set; }
    public bool IsProtected { get; set; }
    public DateTime AddedAt { get; set; }
}
