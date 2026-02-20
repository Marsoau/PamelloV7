using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.DTO;

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
