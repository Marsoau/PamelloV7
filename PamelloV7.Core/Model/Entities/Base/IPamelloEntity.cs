using PamelloV7.Core.DTO;

namespace PamelloV7.Core.Model.Entities.Base;

public interface IPamelloEntity
{
    public int Id { get; }
    public string Name { get; set; }

    public IPamelloDTO GetDTO();
    public void Init();
}
