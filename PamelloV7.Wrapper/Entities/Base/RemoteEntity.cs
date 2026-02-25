using PamelloV7.Core.Dto;

namespace PamelloV7.Wrapper.Entities.Base;

public class RemoteEntity<TDtoType> : IRemoteEntity
    where TDtoType : PamelloEntityDto
{
    public int Id => Dto.Id;
    public string Name => Dto.Name;
    
    internal TDtoType Dto { get; set; }
    
    public RemoteEntity(TDtoType dto) {
        Dto = dto;
    }
    
    public Type GetDtoType() => typeof(TDtoType);
}
