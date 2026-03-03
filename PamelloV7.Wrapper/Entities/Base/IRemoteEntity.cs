using PamelloV7.Core.Dto;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Wrapper.Entities.Base;

public interface IRemoteEntity : IDeletableEntity
{
    public string Name { get; }
    internal PamelloEntityDto Dto { get; }

    internal Type GetDtoType();
}
