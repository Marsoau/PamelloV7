using PamelloV7.Framework.Data.Entities.Base;
using PamelloV7.Framework.Platforms;

namespace PamelloV7.Framework.Data.Entities;

public class DatabaseSong : DatabaseEntity
{
    public required string Name { get; set; }
    
    //Url can point to either a pamello file or any internet image
    public required string CoverUrl { get; set; }
    
    //<filename> part of "[this song id]-<filename>.opus" full file name
    public required List<string> Associations { get; set; }
    
    //key - Service, value - ID
    public required int SelectedSource { get; set; }
    public required List<PlatformKey> Sources { get; set; }
    public required int AddedBy { get; set; }
    public required DateTime AddedAt { get; set; }
}
