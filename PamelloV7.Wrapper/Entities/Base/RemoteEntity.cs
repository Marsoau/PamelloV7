using PamelloV7.Core.Dto;

namespace PamelloV7.Wrapper.Entities.Base;

public class RemoteEntity<TDtoType> : IRemoteEntity
    where TDtoType : PamelloEntityDto
{
    public int Id => Dto.Id;
    public string Name => Dto.Name;
    
    public bool IsDeleted { get; set; }
    
    internal TDtoType Dto { get; set; }
    PamelloEntityDto IRemoteEntity.Dto => Dto;
    
    public RemoteEntity(TDtoType dto) {
        Dto = dto;
    }
    
    public Type GetDtoType() => typeof(TDtoType);

    public override string ToString() {
        return $"[{Id}] {Name}";
    }
}
