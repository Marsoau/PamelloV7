using System.Collections;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Containers;

public interface ISafeStoredEntities
{
    public IEnumerable<int> InternalIds { get; set; }
    public IEnumerable<IPamelloEntity?> InternalEntities { get; set; }
    public IEnumerable<IPamelloEntity> Entities { get; }
    public Type EntitiesType { get; }
}
