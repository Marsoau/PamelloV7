using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Framework.Containers;

public interface ISafeStoredEntity
{
    int Id { get; }
    public IDeletableEntity? Entity { get; set; }
    public Type EntityType { get; }
    public IDeletableEntity RequiredEntity { get; }
}
