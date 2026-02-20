using PamelloV7.Framework.Data.Entities.Base;

namespace PamelloV7.Framework.Data.Entities;

public class DatabasePlaylist : DatabaseEntity
{
    public string Name { get; set; }
    public List<int> SongIds { get; set; }
    public int OwnerId { get; set; }
    public bool IsProtected { get; set; }
    public DateTime AddedAt { get; set; }
}
