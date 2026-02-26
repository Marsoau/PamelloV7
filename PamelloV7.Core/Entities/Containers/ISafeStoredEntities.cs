using System.Collections;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Framework.Containers;

public interface ISafeStoredEntities
{
    public IEnumerable<int> InternalIds { get; set; }
    public IEnumerable<IDeletableEntity?> InternalEntities { get; set; }
    public IEnumerable<IDeletableEntity> Entities { get; }
    public Type EntitiesType { get; }
}
