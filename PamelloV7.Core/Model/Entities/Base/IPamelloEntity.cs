using PamelloV7.Core.DTO;

namespace PamelloV7.Core.Model.Entities.Base;

public interface IPamelloEntity
{
    public int Id { get; }
    public string Name { get; set; }

    public IPamelloDTO GetDto();

    public static IEnumerable<int> GetIds(IEnumerable<IPamelloEntity> entities)
        => entities.Select(entity => entity.Id);
}
