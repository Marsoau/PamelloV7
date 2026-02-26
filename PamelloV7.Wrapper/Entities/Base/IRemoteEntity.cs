using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Wrapper.Entities.Base;

public interface IRemoteEntity : IDeletableEntity
{
    public string Name { get; }

    internal Type GetDtoType();
}
