using PamelloV7.Core.Dto;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.DTO;

namespace PamelloV7.Framework.Entities.Base;

public interface IPamelloEntity : IDeletableEntity
{
    public string Name { get; }
    
    public PamelloEntityDto GetDto();

    public static IEnumerable<int> GetIds(IEnumerable<IPamelloEntity> entities)
        => entities.Select(entity => entity.Id);
}
