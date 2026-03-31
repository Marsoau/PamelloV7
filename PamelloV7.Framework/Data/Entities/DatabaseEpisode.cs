using PamelloV7.Framework.Data.Entities.Base;

namespace PamelloV7.Framework.Data.Entities;

public class DatabaseEpisode : DatabaseEntity
{
    public required string Name { get; set; }
    public required int SongId { get; set;}
    public required int StartSeconds { get; set;}
    public required bool AutoSkip { get; set;}
}
