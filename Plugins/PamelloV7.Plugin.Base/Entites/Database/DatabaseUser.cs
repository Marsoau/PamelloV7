using PamelloV7.Core.Attributes;

namespace PamelloV7.Plugin.Base.Entites.Database;

public record DatabaseUser
{
    [DatabaseEntityId]
    public int Id { get; set; }
    
    public string Name { get; set; }
}
