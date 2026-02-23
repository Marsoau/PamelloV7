using PamelloV7.Core.Dto;
using PamelloV7.Framework.DTO;

namespace PamelloV7.Framework.Entities.Base;

public interface IPamelloEntity
{
    public int Id { get; }
    public string Name { get; }
    
    public bool IsDeleted { get; set; }

    public PamelloEntityDto GetDto();

    public static IEnumerable<int> GetIds(IEnumerable<IPamelloEntity> entities)
        => entities.Select(entity => entity.Id);
}
