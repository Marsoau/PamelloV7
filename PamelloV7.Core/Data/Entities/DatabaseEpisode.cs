using PamelloV7.Core.Data.Entities.Base;

namespace PamelloV7.Core.Data.Entities;

public class DatabaseEpisode : DatabaseEntity
{
    public string Name { get; set; }
    public int SongId { get; set;}
    public int StartSeconds { get; set;}
    public bool AutoSkip { get; set;}
}
