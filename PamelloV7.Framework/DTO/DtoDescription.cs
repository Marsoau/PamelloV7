using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.DTO;

public class DtoDescription
{
    public string Type { get; set; }
    public object Data { get; set; }
    
    public DtoDescription(IPamelloEntity entity)
    {
        var interfaces = entity.GetType().GetInterfaces();
        
        Type = entity.GetType()
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.CustomAttributes.Any(a => a.AttributeType == typeof(ValueEntityAttribute)) &&
                i.IsAssignableTo(typeof(IPamelloEntity))
            )?.Name ?? "";
        Data = entity.GetDto();
    }
}
