using PamelloV7.Core.Data.Entities.Base;

namespace PamelloV7.Core.Data.Entities;

public class DatabaseSong : DatabaseEntity
{
    public string Name { get; set; }
    
    //Url can point to either a pamello file or any internet image
    public string CoverUrl { get; set; }
    public List<string> Associations { get; set; }
    
    //key - Service, value - ID
    public Dictionary<string, string> Sources { get; set; }
    public int AddedBy { get; set; }
    public DateTime AddedAt { get; set; }
    public bool IsSoftDeleted { get; set; }
}
