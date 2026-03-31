using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Dto;

public class DtoDescription
{
    public int EntityId { get; set; }
    public string EntityType { get; set; }
    public object Data { get; set; }
    
    public DtoDescription(IPamelloEntity entity)
    {
        var interfaces = entity.GetType().GetInterfaces();
        
        EntityId = entity.Id;
        EntityType = entity.GetType()
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.CustomAttributes.Any(a => a.AttributeType == typeof(PamelloEntityAttribute)) &&
                i.IsAssignableTo(typeof(IPamelloEntity))
            )?.Name ?? "";
        Data = entity.GetDto();
    }
}
