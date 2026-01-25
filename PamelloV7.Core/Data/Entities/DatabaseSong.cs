using PamelloV7.Core.Data.Entities.Base;
using PamelloV7.Core.Platforms;

namespace PamelloV7.Core.Data.Entities;

public class DatabaseSong : DatabaseEntity
{
    public string Name { get; set; }
    
    //Url can point to either a pamello file or any internet image
    public string CoverUrl { get; set; }
    
    //<filename> part of "[this song id]-<filename>.opus" full file name
    public List<string> Associations { get; set; }
    
    //key - Service, value - ID
    public int SelectedSource { get; set; }
    public List<PlatformKey> Sources { get; set; }
    public int AddedBy { get; set; }
    public DateTime AddedAt { get; set; }
    public bool IsSoftDeleted { get; set; }
}
