using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.DTO;

public class DtoDescription
{
    public string Type { get; set; }
    public object Dto { get; set; }
    
    public DtoDescription(IPamelloEntity entity)
    {
        Type = entity.GetType()
            .GetInterfaces()
            .FirstOrDefault(i => i != typeof(IPamelloEntity) && i.IsAssignableTo(typeof(IPamelloEntity)))?.Name ?? "";
        Dto = entity.GetDto();
    }
}
