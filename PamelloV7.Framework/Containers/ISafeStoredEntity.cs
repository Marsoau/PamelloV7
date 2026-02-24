using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Containers;

public interface ISafeStoredEntity
{
    int Id { get; }
    public IPamelloEntity? Entity { get; }
    public Type EntityType { get; }
    public IPamelloEntity RequiredEntity { get; }
}
