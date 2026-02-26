using PamelloV7.Core.Dto;

namespace PamelloV7.Wrapper.Entities.Base;

public class PamelloEntity<TDtoType> : IPamelloEntity
    where TDtoType : PamelloEntityDto
{
    public int Id => Dto.Id;
    public string Name => Dto.Name;
    
    internal TDtoType Dto { get; set; }
    
    public PamelloEntity(TDtoType dto) {
        Dto = dto;
    }
    
    public Type GetDtoType() => typeof(TDtoType);
}
