using PamelloV7.Core.Data.Entities.Base;

namespace PamelloV7.Core.Data.Entities;

public class DatabaseSong : DatabaseEntity
{
    public string Name { get; set; }
    public string CoverUrl { get; set; }
    public IEnumerable<string> Associations { get; set; }
    public IEnumerable<string> Sources { get; set; }
    public int PlayCount { get; set; }
    public int AddedBy { get; set; }
    public DateTime AddedAt { get; set; }
}
